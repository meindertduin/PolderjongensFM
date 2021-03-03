using System.Collections.Generic;
using Pjfm.Application.AppContexts.Tracks;

namespace Pjfm.Api.Services.SpotifyPlayback.FillerQueueState
{
    public class FillerQueueStateBase
    {
        internal List<TrackDto> RecentlyPlayed = new List<TrackDto>(); 
        public virtual void AddRecentlyPlayed(TrackDto track)
        {
            RecentlyPlayed.Add(track);
        }

        public virtual void Reset()
        {
            RecentlyPlayed = new List<TrackDto>();
        }

        public virtual int GetRecentlyPlayedAmount()
        {
            return RecentlyPlayed.Count;
        }
    }
}