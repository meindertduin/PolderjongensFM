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
        private List<TrackRequestDto> tracksBuffer = new List<TrackRequestDto>();

        private const int MaxRequestsPerUserAmount = 3;

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
            if (tracksBuffer
                .Select(t => t.User.Id)
                .Count(t => t == user.Id) <= MaxRequestsPerUserAmount)
            {
                tracksBuffer.Add(new TrackRequestDto()
                {
                    Track = track,
                    User = user,
                });
                
                return Response.Ok("Nummer toegevoegd aan de wachtrij", true);
            }
            
            return Response.Fail($"U heeft al het maximum van {MaxRequestsPerUserAmount} verzoekjes opgegeven, probeer het later opnieuw", false);
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
            if (tracksBuffer.Count > 0)
            {
                var randomIndex = _random.Next(tracksBuffer.Count);
                _playbackQueue.AddSecondaryTrack(tracksBuffer[randomIndex]);
                tracksBuffer.RemoveAt(randomIndex);
            }
        }
    }
}