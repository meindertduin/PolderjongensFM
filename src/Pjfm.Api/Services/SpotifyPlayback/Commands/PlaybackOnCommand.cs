using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackOnCommand : ICommand
    {
        private readonly IPlaybackController _playbackController;
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private readonly IPlaybackQueue _playbackQueue;

        public PlaybackOnCommand(IPlaybackController playbackController, 
            ISpotifyPlaybackManager spotifyPlaybackManager,
            IPlaybackQueue playbackQueue)
        {
            _playbackController = playbackController;
            _spotifyPlaybackManager = spotifyPlaybackManager;
            _playbackQueue = playbackQueue;
        }
        
        public void Execute()
        {
            _spotifyPlaybackManager.StartPlayingTracks();
            _playbackController.SetPlaybackState(new UserRequestPlaybackState(_playbackQueue));
        }

        public void Undo()
        {
            _spotifyPlaybackManager.StopPlayback(0);
        }
    }
}