using Pjfm.Application.Common.Dto;

namespace Pjfm.WebClient.Services
{
    public class DefaultPlaybackState : IPlaybackState
    {
        private readonly IPlaybackQueue _playbackQueue;

        public DefaultPlaybackState(IPlaybackQueue playbackQueue)
        {
            _playbackQueue = playbackQueue;
        }

        public void AddPriorityTrack(TrackDto track)
        {
            _playbackQueue.AddPriorityTrack(track);
        }

        public void AddSecondaryTrack(TrackDto track)
        {
            // dat er niks hier staat hoort zo 
        }
    }
}