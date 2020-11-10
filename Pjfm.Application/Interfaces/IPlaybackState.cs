using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackState
    {
        Response<bool> AddPriorityTrack(TrackDto track);
        Response<bool> AddSecondaryTrack(TrackDto track);
    }
}