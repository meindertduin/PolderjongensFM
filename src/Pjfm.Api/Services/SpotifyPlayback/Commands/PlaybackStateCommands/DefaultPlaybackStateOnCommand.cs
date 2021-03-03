using Pjfm.Api.Services.SpotifyPlayback.PlaybackStates;
using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Interfaces;

namespace Pjfm.Api.Services.SpotifyPlayback.Commands.PlaybackStateCommands
{
    public class DefaultPlaybackStateOnCommand : ICommand
    {
        private readonly IPlaybackController _playbackController;
        private readonly IPlaybackQueue _playbackQueue;

        public DefaultPlaybackStateOnCommand(IPlaybackController playbackController, IPlaybackQueue playbackQueue)
        {
            _playbackController = playbackController;
            _playbackQueue = playbackQueue;
        }
        public void Execute()
        {
            _playbackController.SetPlaybackState(new DefaultPlaybackState(_playbackQueue));
        }

        public void Undo()
        {
            
        }
    }
}