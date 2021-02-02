using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Domain.Interfaces;
using pjfm.Hubs;
using pjfm.Models;
using Serilog;

namespace Pjfm.WebClient.Services
{
    public class PlaybackListenerManager : IPlaybackListenerManager
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private readonly IPlaybackController _playbackController;
        private readonly IHubContext<RadioHub> _radioHubContext;

        public static readonly ConcurrentDictionary<string, ConnectedUser> ConnectedUsers 
            = new ConcurrentDictionary<string, ConnectedUser>();

        public static readonly ConcurrentDictionary<string, TimedListenerModel> SubscribedListeners = new ConcurrentDictionary<string, TimedListenerModel>();
        
        
        public PlaybackListenerManager(ISpotifyPlaybackManager spotifyPlaybackManager, IPlaybackController playbackController, 
            IHubContext<RadioHub> radioHubContext)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
            _playbackController = playbackController;
            _radioHubContext = radioHubContext;
        }
        
        public async Task AddListener(ApplicationUser user, PlaybackDevice playbackDevice)
        {
            // add user to ConnectedUsers if not null and synch with player
            if (user != null)
            {
                ConnectedUsers[user.Id] = new ConnectedUser()
                {
                    User = user,
                    PlaybackDevice = playbackDevice,
                };
            
                await _playbackController.SynchWithPlayback(user.Id, user.SpotifyAccessToken, playbackDevice);
            }
        }

        public bool IsUserTimedListener(string userId)
        {
            return SubscribedListeners.ContainsKey(userId);
        }

        public int? GetUserSubscribeTime(string userId)
        {
            var getResult = SubscribedListeners.TryGetValue(userId, out var subscribedUserInfo);
            if (getResult)
            {
                return subscribedUserInfo.SubscribeTimeMinutes;
            }
            
            return null;
        }
        
        
        public ApplicationUser RemoveListener(string userId)
        {
            ConnectedUsers.TryRemove(userId, out ConnectedUser connectedUser);

            return connectedUser?.User;
        }

        public bool TrySetTimedListener(string userId, int minutes, string userConnectionId)
        {
            // handle if user already has timed session
            if (SubscribedListeners.ContainsKey(userId))
            {
                var removeResult = SubscribedListeners.TryRemove(userId, out TimedListenerModel userPreviousSubscribeSession);
                
                // cancel previous timed listener session if already one was active
                if (removeResult)
                {
                    userPreviousSubscribeSession.TimedListenerCancellationTokenSource.Cancel();
                }
                else
                {
                    return false;
                }
            }
            
            // register stopping token to be able to cancel a timed listen session
            var stoppingTokenSource = new CancellationTokenSource();

            var addResult = SubscribedListeners.TryAdd(userId, new TimedListenerModel()
            {
                TimeAdded = DateTime.Now,
                SubscribeTimeMinutes = minutes,
                ConnectionId = userConnectionId,
                TimedListenerCancellationTokenSource = stoppingTokenSource,
            });

            if (addResult)
            {
                var stoppingToken = stoppingTokenSource.Token;
                
                // run timed event scheduled on the thread pool
                Task.Run(() => RunTimedEvent(userId, minutes, stoppingToken), stoppingToken);
                return true;
            }

            return false;
        }

        public bool TryRemoveTimedListener(string userId)
        {
            var removeResult = SubscribedListeners.TryRemove(userId, out TimedListenerModel subscribedListenerInfo);
            
            if (removeResult)
            {
                RemoveListener(userId);
                // cancel users timed listen session
                subscribedListenerInfo.TimedListenerCancellationTokenSource.Cancel();
                
                try
                {
                    // update users playback connected status
                    _radioHubContext.Clients.Client(subscribedListenerInfo.ConnectionId)
                        .SendAsync("IsConnected", false);
                }
                catch (Exception e)
                {
                    Log.Warning(e.Message);
                }
                
                return true;
            }

            return false;
        }
        
        private async Task RunTimedEvent(string userId, int minutes, CancellationToken stopToken)
        {
            // removes timed listener after specified minutes
            await Task.Delay(minutes * 60_000, stopToken);
            TryRemoveTimedListener(userId);
        }
    }
}