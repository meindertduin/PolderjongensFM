using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackModeMediumLongTermCommand : ICommand
    {
        private readonly IPlaybackQueue _playbackQueue;
        private TopTrackTermFilter _previousTermFilter;

        public PlaybackModeMediumLongTermCommand(IPlaybackQueue playbackQueue)
        {
            _playbackQueue = playbackQueue;
        } 
        
        public void Execute()
        {
            _previousTermFilter = _playbackQueue.CurrentTermFilter;
            _playbackQueue.SetTermFilter(TopTrackTermFilter.MediumLongTerm);
        }

        public void Undo()
        {
            _playbackQueue.SetTermFilter(_previousTermFilter);
        }
    }
}