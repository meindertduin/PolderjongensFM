using System;
using System.Collections.Generic;
using System.Linq;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using pjfm.Models;

namespace Pjfm.WebClient.Services
{
    public class RandomRequestPlaybackState : IPlaybackState, IObserver<bool>
    {
        private readonly IPlaybackQueue _playbackQueue;
        private readonly IPlaybackController _playbackController;
        private IDisposable _unsubscriber;
        private List<TrackRequestDto> _tracksBuffer = new List<TrackRequestDto>();

        private  int _maxRequestsPerUserAmount = 3;

        private bool _hasNoSecondary = true;

        private Random _random = new Random();

        public RandomRequestPlaybackState(IPlaybackController playbackController, IPlaybackQueue playbackQueue)
        {
            _playbackQueue = playbackQueue;
            _playbackController = playbackController;

            _unsubscriber = _playbackController.SubscribeToPlayingStatus(this);

        }
        public Response<bool> AddPriorityTrack(TrackDto track)
        {
            _playbackQueue.AddPriorityTrack(track);
            
            return Response.Ok("Nummer toegevoegd aan de wachtrij", true);
        }

        public Response<bool> AddSecondaryTrack(TrackDto track, ApplicationUserDto user)
        {
            // if user doesn't exceed max tracks amount add new track as request
            if (_tracksBuffer
                .Select(t => t.User.Id)
                .Count(t => t == user.Id) < _maxRequestsPerUserAmount)
            {
                // add directly to playbackQueue if there are no tracks in the buffer
                if (_hasNoSecondary)
                {
                    _playbackQueue.AddSecondaryTrack(new TrackRequestDto()
                    {
                        Track = track,
                        User = user,
                    });
                    
                    _hasNoSecondary = false;
                }
                // add to buffer
                else
                {
                    _tracksBuffer.Add(new TrackRequestDto()
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
            var result = new List<TrackDto>();

            foreach (var request in _tracksBuffer)
            {
                var trackDto = request.Track;
                trackDto.User = request.User;

                result.Add(trackDto);
            }
            
            return result;
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
            if (_tracksBuffer.Count > 0)
            {
                // add a random track from the tracksBuffer to the playbackQueue
                var randomIndex = _random.Next(_tracksBuffer.Count);
                _playbackQueue.AddSecondaryTrack(_tracksBuffer[randomIndex]);
                _tracksBuffer.RemoveAt(randomIndex);
            }
            else
            {
                _hasNoSecondary = true;
            }
        }
    }
}