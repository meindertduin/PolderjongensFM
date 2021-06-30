using System.Collections.Generic;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;

namespace Pjfm.WebClient.Services.FillerQueueState
{
    public interface IFillerQueueState
    {
        void AddRecentlyPlayed(TrackDto track);
        void Reset();
        int GetRecentlyPlayedAmount();
        Task<List<TrackDto>> RetrieveFillerTracks(int amount);
    }
}