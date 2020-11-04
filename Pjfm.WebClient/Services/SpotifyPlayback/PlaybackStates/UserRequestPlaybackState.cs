using Pjfm.Application.Common.Dto;

namespace Pjfm.WebClient.Services
{
    public class UserRequestPlaybackState : IPlaybackState
    {
        private readonly IPlaybackQueue _playbackQueue;

        public UserRequestPlaybackState(IPlaybackQueue playbackQueue)
        {
            _playbackQueue = playbackQueue;
        }
        
        public void AddPriorityTrack(TrackDto track)
        {
            _playbackQueue.AddPriorityTrack(track);
        }

        public void AddSecondaryTrack(TrackDto track)
        {
            _playbackQueue.AddSecondaryTrack(track);
        }
    }
}