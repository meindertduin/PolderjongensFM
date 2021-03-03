using System.Collections.Generic;
using Pjfm.Application.AppContexts.Tracks;
using Pjfm.Application.AppContexts.Users;
using Pjfm.Application.Common.Mediatr;

namespace Pjfm.Application.Interfaces
{
    public interface IPlaybackState
    {
        Response<bool> AddPriorityTrack(TrackDto track);
        Response<bool> AddSecondaryTrack(TrackDto track, ApplicationUserDto user);
        List<TrackDto> GetSecondaryTracks();
        void SetMaxRequestsPerUser(int amount);
        int GetMaxRequestsPerUser();
    }
}