using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
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
            _spotifyPlaybackManager.StopPlayingTracks(0);
        }

        public void Undo()
        {
            _spotifyPlaybackManager.StartPlayingTracks();
        }
    }
}