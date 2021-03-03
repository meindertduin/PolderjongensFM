using Pjfm.Api.Services.SpotifyPlayback.PlaybackStates;
using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Interfaces;

namespace Pjfm.Api.Services.SpotifyPlayback.Commands.PlaybackStateCommands
{
    public class UserRequestPlaybackStateOnCommand : ICommand
    {
        private readonly IPlaybackController _playbackController;
        private readonly IPlaybackQueue _playbackQueue;

        public UserRequestPlaybackStateOnCommand(IPlaybackController playbackController, IPlaybackQueue playbackQueue)
        {
            _playbackController = playbackController;
            _playbackQueue = playbackQueue;
        }
        public void Execute()
        {
            _playbackController.SetPlaybackState(new UserRequestPlaybackState(_playbackQueue));   
        }

        public void Undo()
        {
            _playbackController.SetPlaybackState(new DefaultPlaybackState(_playbackQueue));   
        }
    }
}