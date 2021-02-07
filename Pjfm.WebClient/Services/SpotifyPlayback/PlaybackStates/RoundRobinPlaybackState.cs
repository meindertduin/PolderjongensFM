using System;
using System.Collections.Generic;
using System.Linq;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Domain.Enums;
using pjfm.Models;

namespace Pjfm.WebClient.Services
{
    public class RoundRobinPlaybackState : IPlaybackState, IObserver<bool>
    {
        private readonly IPlaybackQueue _playbackQueue;
        private  int _maxRequestsPerUserAmount = 3;

        private Dictionary<string, List<(TrackDto Track, ApplicationUserDto User)>> _secondaryRequests = 
            new Dictionary<string, List<(TrackDto Track, ApplicationUserDto User)>>();

        private bool _secondaryInQueue = false;

        public RoundRobinPlaybackState(IPlaybackQueue playbackQueue)
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
            var hasDoneRequests = _secondaryRequests.TryGetValue(user.Id, out var requests);
            
            if (hasDoneRequests == false || requests == null)
            {
                requests = new List<(TrackDto Track, ApplicationUserDto User)>();
            }
            
            if (requests.Count < _maxRequestsPerUserAmount)
            {
                if (_secondaryInQueue == false)
                {
                    _playbackQueue.AddSecondaryTrack(new TrackRequestDto()
                    {
                        Track = track,
                        User = user,
                    });
                }
                else
                {
                    requests.Add((track, user));
                    _secondaryRequests[user.Id] = requests;   
                }
                    
                return Response.Ok("Nummer toegevoegd aan de wachtrij", true);
            }
            
            return Response.Fail($"U heeft al het maximum van {_maxRequestsPerUserAmount} verzoekjes opgegeven, probeer het later opnieuw", false);
        }

        public List<TrackDto> GetSecondaryTracks()
        {
            var result = new List<TrackDto>();

            foreach (var request in _secondaryRequests)
            {
                result.AddRange(request.Value.Select(x =>
                {
                    var trackDto = x.Track;
                    trackDto.User = x.User;
                    return trackDto;
                }));
            }
    
            // this block of linq queries makes the result round robin
            return result
                .GroupBy(track => track.User.Id)
                .SelectMany((trackGroup, groupIndex) =>
                    trackGroup.Select((item, index) => new
                    {
                        Index = index,
                        GroupIndex = groupIndex,
                        Value = item,
                    }))
                .OrderBy(u => u.Index)
                .ThenBy(u => u.GroupIndex)
                .Select((u) => u.Value)
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
                var track = GetSecondaryTracks()[0];

                var result = _secondaryRequests.TryGetValue(track.User.Id, out var requests);

                if (result && requests != null)
                {
                    var request = requests.FirstOrDefault(x => x.Track.Id == track.Id);
                    _playbackQueue.AddSecondaryTrack(new TrackRequestDto()
                    {
                        User = request.User,
                        Track = request.Track,
                    });

                    if (requests.Count <= 1)
                    {
                        _secondaryRequests.Remove(request.User.Id);
                    }
                    else
                    {
                        requests.Remove(request);
                    }
                }
            }
            else
            {
                _secondaryInQueue = false;
            }
        }
    }
}