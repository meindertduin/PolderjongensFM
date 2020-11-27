using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Pjfm.Application.Identity;
using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackListenerManager : IPlaybackListenerManager
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private readonly IPlaybackController _playbackController;

        public static readonly ConcurrentDictionary<string, ApplicationUser> ConnectedUsers 
            = new ConcurrentDictionary<string, ApplicationUser>();

        public static readonly ConcurrentDictionary<string, CancellationTokenSource> TimedUsers = new ConcurrentDictionary<string, CancellationTokenSource>();
        
        public PlaybackListenerManager(ISpotifyPlaybackManager spotifyPlaybackManager, IPlaybackController playbackController)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
            _playbackController = playbackController;
        }
        
        public async Task AddListener(ApplicationUser user)
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

        public bool IsUserTimedListener(string userId)
        {
            return TimedUsers.ContainsKey(userId);
        }
        
        public async Task<ApplicationUser> RemoveListener(string userId)
        {
            ConnectedUsers.TryRemove(userId, out ApplicationUser user);
            
            return user;
        }

        public bool TrySetTimedListener(string userId, int minutes)
        {
            if (TimedUsers.ContainsKey(userId))
            {
                var removeResult = TimedUsers.TryRemove(userId, out CancellationTokenSource inUseStoppingToken);
                if (removeResult)
                {
                    inUseStoppingToken.Cancel();
                }
                else
                {
                    return false;
                }
            }
            
            var stoppingTokenSource = new CancellationTokenSource();

            var addResult = TimedUsers.TryAdd(userId, stoppingTokenSource);

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
            var removeResult = TimedUsers.TryRemove(userId, out CancellationTokenSource stoppingTokenSource);
            if (removeResult)
            {
                stoppingTokenSource.Cancel();
                return true;
            }

            return false;
        }

        private async Task RunTimedEvent(string userId,int minutes, CancellationToken stopToken)
        {
            await Task.Delay(minutes * 60_000, stopToken);
            await RemoveListener(userId);
        }
    }
}