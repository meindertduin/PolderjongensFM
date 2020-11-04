using Pjfm.Application.Common.Dto;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackState
    {
        void AddPriorityTrack(TrackDto track);
        void AddSecondaryTrack(TrackDto track);
    }
}