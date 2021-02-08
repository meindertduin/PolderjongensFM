using System;
using System.Collections.Generic;
using System.Linq;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.Domain.Enums;
using pjfm.Models;

namespace Pjfm.WebClient.Services
{
    public class RoundRobinPlaybackState : IPlaybackState, IObserver<bool>
    {
        private readonly IPlaybackQueue _playbackQueue;
        private  int _maxRequestsPerUserAmount = 3;

        private RoundRobinTrackRequestDtoList<Queue<TrackRequestDto>> _secondaryRequests = 
            new RoundRobinTrackRequestDtoList<Queue<TrackRequestDto>>();

        private bool _secondaryInQueue = false;
        private IDisposable _unsubscriber;

        public RoundRobinPlaybackState(IPlaybackController playbackController,IPlaybackQueue playbackQueue)
        {
            _playbackQueue = playbackQueue;
            
            _unsubscriber = playbackController.SubscribeToPlayingStatus(this);
        }
        public Response<bool> AddPriorityTrack(TrackDto track)
        {
            track.TrackType = TrackType.DjTrack;
            _playbackQueue.AddPriorityTrack(track);
            
            return Response.Ok("Nummer toegevoegd aan de wachtrij", true);
        }

        public Response<bool> AddSecondaryTrack(TrackDto track, ApplicationUserDto user)
        {
            var requestCount = _secondaryRequests.GetRequestsCountUser(user.Id);
            
            
            if (requestCount < _maxRequestsPerUserAmount)
            {
                if (_secondaryInQueue == false)
                {
                    _playbackQueue.AddSecondaryTrack(new TrackRequestDto()
                    {
                        Track = track,
                        User = user,
                    });

                    _secondaryInQueue = true;
                }
                else
                {
                    _secondaryRequests.Add(new TrackRequestDto()
                    {
                        Track = track,
                        User = user,
                    });
                }
                    
                return Response.Ok("Nummer toegevoegd aan de wachtrij", true);
            }
            
            return Response.Fail($"U heeft al het maximum van {_maxRequestsPerUserAmount} verzoekjes opgegeven, probeer het later opnieuw", false);
        }

        public List<TrackDto> GetSecondaryTracks()
        {
            return _secondaryRequests
                .GetValues()
                .Select(x => x.Track)
                .ToList();
        }

        public void SetMaxRequestsPerUser(int amount)
        {
            _maxRequestsPerUserAmount = amount;
        }

        public int GetMaxRequestsPerUser()
        {
            return _maxRequestsPerUserAmount;
        }
        
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
        
        public void OnNext(bool value)
        {
            if (_secondaryRequests.Count > 0)
            {
                var nextRequest = _secondaryRequests.GetNextRequest();

                if (nextRequest != null)
                {
                    _playbackQueue.AddSecondaryTrack(nextRequest);
                }
            }
            else
            {
                _secondaryInQueue = false;
            }
        }
    }
}