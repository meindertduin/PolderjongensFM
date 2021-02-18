using System.Collections.Generic;
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
        private int _maxRequestsPerUserAmount = 3;

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

        public Response<bool> AddSecondaryTrack(TrackDto track, ApplicationUserDto user)
        {
            track.TrackType = TrackType.RequestedTrack;

            var queuedTracks = _playbackQueue.GetSecondaryQueueRequests();
            
            // add track to playbackQueue if user doesn't exceed max request amount
            if (queuedTracks.Select(q => q.User.Id).Count(q => q == user.Id) < _maxRequestsPerUserAmount)
            {
                _playbackQueue.AddSecondaryTrack(new TrackRequestDto()
                {
                    Track = track,
                    User = user,
                });
            
                return Response.Ok("Nummer toegevoegd aan de wachtrij", true);
            }
            
            return Response.Fail($"Je zit op het limiet van {_maxRequestsPerUserAmount} verzoekjes per gebruiker. Probeer het later opnieuw.", false); }

        public List<TrackDto> GetSecondaryTracks()
        {
            return _playbackQueue.GetSecondaryQueueTracks();
        }
        
        public void SetMaxRequestsPerUser(int amount)
        {
            _maxRequestsPerUserAmount = amount;
        }

        public int GetMaxRequestsPerUser()
        {
            return _maxRequestsPerUserAmount;
        }
        
    }
}