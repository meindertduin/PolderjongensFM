using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.Domain.Enums;

namespace Pjfm.WebClient.Services
{
    public class UserRequestPlaybackState : IPlaybackState
    {
        private readonly IPlaybackQueue _playbackQueue;

        public UserRequestPlaybackState(IPlaybackQueue playbackQueue)
        {
            _playbackQueue = playbackQueue;
        }
        
        public Response<bool> AddPriorityTrack(TrackDto track)
        {
            track.TrackType = TrackType.DjTrack;
            _playbackQueue.AddPriorityTrack(track);
            
            return Response.Ok("Nummer toegevoegd aan de wachtrij", true);
        }

        public Response<bool> AddSecondaryTrack(TrackDto track)
        {
            track.TrackType = TrackType.RequestedTrack;
            _playbackQueue.AddSecondaryTrack(track);
            
            return Response.Ok("Nummer toegevoegd aan de wachtrij", true);
        }
    }
}