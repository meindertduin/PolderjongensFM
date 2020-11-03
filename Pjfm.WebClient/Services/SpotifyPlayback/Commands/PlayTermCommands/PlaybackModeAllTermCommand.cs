using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
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