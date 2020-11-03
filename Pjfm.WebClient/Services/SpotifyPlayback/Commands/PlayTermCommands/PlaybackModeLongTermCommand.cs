using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackModeLongTermCommand : ICommand
    {
        private readonly IPlaybackQueue _playbackQueue;
        private TopTrackTermFilter _previousTermFilter;

        public PlaybackModeLongTermCommand(IPlaybackQueue playbackQueue)
        {
            _playbackQueue = playbackQueue;
        } 
        
        public void Execute()
        {
            _previousTermFilter = _playbackQueue.CurrentTermFilter;
            _playbackQueue.SetTermFilter(TopTrackTermFilter.LongTerm);
        }

        public void Undo()
        {
            _playbackQueue.SetTermFilter(_previousTermFilter);
        }
    }
}