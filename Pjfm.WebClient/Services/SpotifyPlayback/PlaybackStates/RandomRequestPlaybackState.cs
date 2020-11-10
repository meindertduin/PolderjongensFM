using System;
using System.Collections.Generic;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;

namespace Pjfm.WebClient.Services
{
    public class RandomRequestPlaybackState : IPlaybackState, IObserver<bool>
    {
        private readonly IPlaybackQueue _playbackQueue;
        private readonly IPlaybackController _playbackController;
        private IDisposable _unsubscriber;
        private List<TrackDto> tracksBuffer = new List<TrackDto>();

        private Random _random = new Random();

        public RandomRequestPlaybackState(IPlaybackQueue playbackQueue, IPlaybackController playbackController)
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

        public Response<bool> AddSecondaryTrack(TrackDto track)
        {
            tracksBuffer.Add(track);
            
            return Response.Ok("Nummer toegevoegd aan de wachtrij", true);
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
            var randomIndex = _random.Next(tracksBuffer.Count);
            _playbackQueue.AddSecondaryTrack(tracksBuffer[randomIndex]);
            tracksBuffer.RemoveAt(randomIndex);
        }
    }
}