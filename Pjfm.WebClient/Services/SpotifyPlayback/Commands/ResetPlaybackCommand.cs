using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class ResetPlaybackCommand : ICommand
    {
        private readonly IPlaybackController _playbackController;
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;

        public ResetPlaybackCommand(IPlaybackController playbackController ,ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _playbackController = playbackController;
            _spotifyPlaybackManager = spotifyPlaybackManager;
        }
        public async void Execute()
        {
            await _spotifyPlaybackManager.ResetPlayingTracks(0);
            _playbackController.ResetPlaybackState();
        }

        public void Undo()
        {
            
        }
    }
}