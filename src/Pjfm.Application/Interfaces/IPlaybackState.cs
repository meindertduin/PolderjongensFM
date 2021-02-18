using System.Collections.Generic;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;

namespace Pjfm.WebClient.Services
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