using System.Collections.Generic;
using System.Threading.Tasks;
using Pjfm.Application.AppContexts.Tracks;
using Pjfm.Application.Common.Mediatr;

namespace Pjfm.Api.Services.SpotifyPlayback.FillerQueueState
{
    public interface IFillerQueueState
    {
        void AddRecentlyPlayed(TrackDto track);
        void Reset();
        int GetRecentlyPlayedAmount();
        Task<Response<List<TrackDto>>> RetrieveFillerTracks(int amount);
    }
}