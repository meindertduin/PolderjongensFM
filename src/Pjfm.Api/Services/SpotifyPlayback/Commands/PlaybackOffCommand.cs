using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Interfaces;

namespace Pjfm.Api.Services.SpotifyPlayback.Commands
{
    public class PlaybackOffCommand : ICommand
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;

        public PlaybackOffCommand(ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
        }
        
        public void Execute()
        {
            _spotifyPlaybackManager.StopPlayback(0);
        }

        public void Undo()
        {
            _spotifyPlaybackManager.StartPlayingTracks();
        }
    }
}