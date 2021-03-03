using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Interfaces;
using Pjfm.Domain.Enums;

namespace Pjfm.Api.Services.SpotifyPlayback.Commands.PlayTermCommands
{
    public class PlaybackModeMediumTermCommand : ICommand
    {
        private readonly IPlaybackQueue _playbackQueue;
        private TopTrackTermFilter _previousTermFilter;

        public PlaybackModeMediumTermCommand(IPlaybackQueue playbackQueue)
        {
            _playbackQueue = playbackQueue;
        } 
        
        public void Execute()
        {
            _previousTermFilter = _playbackQueue.CurrentTermFilter;
            _playbackQueue.SetTermFilter(TopTrackTermFilter.MediumTerm);
        }

        public void Undo()
        {
            _playbackQueue.SetTermFilter(_previousTermFilter);
        }
    }
}