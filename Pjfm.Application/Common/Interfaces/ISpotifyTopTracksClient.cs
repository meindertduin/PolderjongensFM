using System.Collections.Generic;
using System.Threading.Tasks;
using Pjfm.Domain.Entities;

namespace Pjfm.Domain.Interfaces
{
    public interface ISpotifyTopTracksClient
    {
        public Task<List<TopTrack>> GetTopTracks(string accessToken, int term, string userId);
    }
}