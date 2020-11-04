using System.Collections.Generic;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Entities;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackQueue
    {
        public int RecentlyPlayedCount();
        void Reset();
        TopTrackTermFilter CurrentTermFilter { get; }
        void SetTermFilter(TopTrackTermFilter termFilter);
        public void AddPriorityTrack(TrackDto track);
        void AddSecondaryTrack(TrackDto track);
        public List<TrackDto> GetFillerQueueTracks();
        public List<TrackDto> GetPriorityQueueTracks();
        List<TrackDto> GetSecondaryQueueTracks();
        public Task<TrackDto> GetNextQueuedTrack();
        public Task AddToFillerQueue(int amount);
    }
}