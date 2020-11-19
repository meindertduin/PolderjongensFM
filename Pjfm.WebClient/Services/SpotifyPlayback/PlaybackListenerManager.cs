using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
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

        public async Task<ApplicationUser> RemoveListener(string userId)
        {
            ConnectedUsers.TryRemove(userId, out ApplicationUser user);
            
            return user;
        }
    }
}