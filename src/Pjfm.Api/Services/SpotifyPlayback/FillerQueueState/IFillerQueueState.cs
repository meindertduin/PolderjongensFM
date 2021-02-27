using System.Collections.Generic;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;

namespace Pjfm.WebClient.Services.FillerQueueState
{
    public interface IFillerQueueState
    {
        void AddRecentlyPlayed(TrackDto track);
        void Reset();
        int GetRecentlyPlayedAmount();
        Task<Response<List<TrackDto>>> RetrieveFillerTracks(int amount);
    }
}