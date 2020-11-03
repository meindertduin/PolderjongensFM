using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class SpotifyPlaybackManager : ISpotifyPlaybackManager
    {
        private int _fillerQueueLength = 50;
        
        private static readonly List<IObserver<bool>> _observers = new List<IObserver<bool>>();
        
        private TrackTimer _timer;
        private readonly ISpotifyPlayerService _spotifyPlayerService;
        private readonly IPlaybackQueue _playbackQueue;

        public SpotifyPlaybackManager(ISpotifyPlayerService spotifyPlayerService, IPlaybackQueue playbackQueue)
        {
            _spotifyPlayerService = spotifyPlayerService;
            _playbackQueue = playbackQueue;

            _timer = new TrackTimer(this);
        }

        public bool IsCurrentlyPlaying { get; private set; }
        
        public DateTime CurrentTrackStartTime { get; private set; }
        public TopTrack CurrentPlayingTrack { get; private set; }
        
        public async Task StartPlayingTracks()
        {
            IsCurrentlyPlaying = true;
            NotifyObserversPlayingStatus(IsCurrentlyPlaying);
        }

        public async Task StopPlayingTracks(int afterDelay)
        {
            await Task.Delay(afterDelay);
            IsCurrentlyPlaying = false;
            NotifyObserversPlayingStatus(IsCurrentlyPlaying);
            _playbackQueue.Reset();
        }
        
        public async Task ResetPlayer(int afterDelay)
        {
            await StopPlayingTracks(afterDelay);
            await StartPlayingTracks();
        }
        
        public async Task<int> PlayNextTrack()
        {
            if (_playbackQueue.RecentlyPlayedCount() <= 0)
            {
                await _playbackQueue.AddToFillerQueue(_fillerQueueLength);
            }
            
            var nextTrack = await _playbackQueue.GetNextQueuedTrack();
            
            CurrentPlayingTrack = nextTrack;
            CurrentTrackStartTime = DateTime.Now;

            await PlayTrackForAll(nextTrack);
            NotifyObserversPlayingStatus(IsCurrentlyPlaying);
            
            return nextTrack.SongDurationMs;
        }
        
        private async Task PlayTrackForAll(TopTrack track)
        {
            var responseTasks = new List<Task<HttpResponseMessage>>();

            foreach (var keyValuePair in PlaybackListenerManager.ConnectedUsers)
            {
                var playTask = _spotifyPlayerService.Play(keyValuePair.Key, keyValuePair.Value.SpotifyAccessToken, String.Empty,
                    new PlayRequestDto()
                    {
                        Uris = new[] {$"spotify:track:{track.Id}"}
                    });
                responseTasks.Add(playTask);
            }
            
            await Task.WhenAll(responseTasks);
        }
        
        public async Task SynchWithCurrentPlayer(string userId, string accessToken)
        {
            var synchedRequestData = GetSynchronisedRequestData();
            await _spotifyPlayerService.Play(userId, accessToken, String.Empty, synchedRequestData);
        }

        private PlayRequestDto GetSynchronisedRequestData()
        {
            var timeSpan = DateTime.Now - CurrentTrackStartTime;

            var requestInfo = new PlayRequestDto()
            {
                Uris = new[] {$"spotify:track:{CurrentPlayingTrack.Id}"},
                PositionMs = (int) timeSpan.TotalMilliseconds,
            };
            
            return requestInfo;
        }
        
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            if (_observers.Contains(observer) == false)
            {
                _observers.Add(observer);
            }
            return new UnSubscriber(_observers, observer);
        }

        private class UnSubscriber : IDisposable
        {
            private List<IObserver<bool>>_observers;
            private IObserver<bool> _observer;

            public UnSubscriber(List<IObserver<bool>> observers, IObserver<bool> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void NotifyObserversPlayingStatus(bool isPlaying)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(isPlaying);
            }
        }
    }
}