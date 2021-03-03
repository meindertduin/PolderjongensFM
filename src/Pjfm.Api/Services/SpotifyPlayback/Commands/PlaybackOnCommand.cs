using Pjfm.Api.Services.SpotifyPlayback.PlaybackStates;
using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Interfaces;

namespace Pjfm.Api.Services.SpotifyPlayback.Commands
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