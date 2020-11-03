using System.Collections.Generic;
using System.Threading.Tasks;
using Pjfm.Domain.Entities;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackQueue
    {
        public int RecentlyPlayedCount();
        void Reset();
        TopTrackTermFilter CurrentTermFilter { get; }
        void SetTermFilter(TopTrackTermFilter termFilter);
        public void AddPriorityTrack(TopTrack track);
        public List<TopTrack> GetFillerQueueTracks();
        public List<TopTrack> GetPriorityQueueTracks();
        public Task<TopTrack> GetNextQueuedTrack();
        public Task AddToFillerQueue(int amount);
    }
}