using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Interfaces;
using Pjfm.Domain.Enums;

namespace Pjfm.Api.Services.SpotifyPlayback.Commands.PlayTermCommands
{
    public class PlaybackModeAllTermCommand : ICommand
    {
        private readonly IPlaybackQueue _playbackQueue;
        private TopTrackTermFilter _previousTermFilter;

        public PlaybackModeAllTermCommand(IPlaybackQueue playbackQueue)
        {
            _playbackQueue = playbackQueue;
        } 
        
        public void Execute()
        {
            _previousTermFilter = _playbackQueue.CurrentTermFilter;
            _playbackQueue.SetTermFilter(TopTrackTermFilter.AllTerms);
        }

        public void Undo()
        {
            _playbackQueue.SetTermFilter(_previousTermFilter);
        }
    }
}