using System.Linq;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.Domain.Enums;
using pjfm.Models;

namespace Pjfm.WebClient.Services
{
    public class UserRequestPlaybackState : IPlaybackState
    {
        private readonly IPlaybackQueue _playbackQueue;
        private const int MaxRequestPerUserAmount = 3;
        
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

        public Response<bool> AddSecondaryTrack(TrackDto track, string userId)
        {
            track.TrackType = TrackType.RequestedTrack;

            var queuedTracks = _playbackQueue.GetSecondaryQueueRequests();
            
            if (queuedTracks.Select(q => q.UserId).Count(q => q == userId) < MaxRequestPerUserAmount)
            {
                _playbackQueue.AddSecondaryTrack(new TrackRequestDto()
                {
                    Track = track,
                    UserId = userId,
                });
            
                return Response.Ok("Nummer toegevoegd aan de wachtrij", true);
            }
            
            return Response.Fail($"U heeft al het maximum van {MaxRequestPerUserAmount} voerzoekjes opgegeven, probeer het later opnieuw", false); }
    }
}