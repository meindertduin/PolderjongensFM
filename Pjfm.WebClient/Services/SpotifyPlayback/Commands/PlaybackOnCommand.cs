using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackOnCommand : ICommand
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;

        public PlaybackOnCommand(ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
        }
        
        public void Execute()
        {
            _spotifyPlaybackManager.StartPlayingTracks();
        }

        public void Undo()
        {
            _spotifyPlaybackManager.StopPlayingTracks(0);
        }
    }
}