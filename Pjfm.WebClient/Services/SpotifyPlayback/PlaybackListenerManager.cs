using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
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

        public static readonly ConcurrentDictionary<string, ApplicationUser> ConnectedUsers 
            = new ConcurrentDictionary<string, ApplicationUser>();

        public static readonly ConcurrentDictionary<string, TimedListenerModel> SubscribedListeners = new ConcurrentDictionary<string, TimedListenerModel>();
        
        
        public PlaybackListenerManager(ISpotifyPlaybackManager spotifyPlaybackManager, IPlaybackController playbackController, 
            IHubContext<RadioHub> radioHubContext)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
            _playbackController = playbackController;
            _radioHubContext = radioHubContext;
        }
        
        public async Task AddListener(ApplicationUser user)
        {
            if (user != null)
            {
                ConnectedUsers[user.Id] = user;
            
                if (_spotifyPlaybackManager.IsCurrentlyPlaying == false)
                {
                    _playbackController.TurnOn(PlaybackControllerCommands.TrackPlayerOnOff);
                }
                else
                {
                    await _playbackController.SynchWithPlayback(user.Id, user.SpotifyAccessToken);
                }
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
            ConnectedUsers.TryRemove(userId, out ApplicationUser user);
            
            return user;
        }

        public bool TrySetTimedListener(string userId, int minutes, string userConnectionId)
        {
            if (SubscribedListeners.ContainsKey(userId))
            {
                var removeResult = SubscribedListeners.TryRemove(userId, out TimedListenerModel userPreviousSubscribeSession);
                if (removeResult)
                {
                    userPreviousSubscribeSession.TimedListenerCancellationTokenSource.Cancel();
                }
                else
                {
                    return false;
                }
            }
            
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
                subscribedListenerInfo.TimedListenerCancellationTokenSource.Cancel();
                
                try
                {
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
            await Task.Delay(minutes * 60_000, stopToken);
            TryRemoveTimedListener(userId);
        }
    }
}