using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR.Users.Queries;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class SpotifyPlaybackManager : ISpotifyPlaybackManager
    {
        private int _fillerQueueLength = 50;
        
        private static readonly List<IObserver<bool>> _observers = new List<IObserver<bool>>();
        
        private readonly ISpotifyPlayerService _spotifyPlayerService;
        private readonly IPlaybackQueue _playbackQueue;
        private readonly IServiceProvider _serviceProvider;

        private Timer _trackTimer;
        private AutoResetEvent _trackTimerAutoEvent;
        private bool _isCurrentlyPlaying;

        public SpotifyPlaybackManager(ISpotifyPlayerService spotifyPlayerService, 
            IPlaybackQueue playbackQueue, IServiceProvider serviceProvider)
        {
            _spotifyPlayerService = spotifyPlayerService;
            _playbackQueue = playbackQueue;
            _serviceProvider = serviceProvider;
        }

        bool ISpotifyPlaybackManager.IsCurrentlyPlaying
        {
            get => _isCurrentlyPlaying;
            set => _isCurrentlyPlaying = value;
        }

        public DateTime CurrentTrackStartTime { get; private set; }
        public TrackDto CurrentPlayingTrack { get; private set; }
        
        public async Task StartPlayingTracks()
        {
            _isCurrentlyPlaying = true;
            await _playbackQueue.SetIncludedUsers();
            
            var nextTrackDuration = await PlayNextTrack();
            CreateTimer();
            
            _trackTimer.Change(nextTrackDuration, nextTrackDuration);
            _trackTimerAutoEvent.WaitOne();
        }

        public async Task ResetPlayingTracks(int afterDelay)
        {
            await StopPlayingTracks(afterDelay);
            
            _isCurrentlyPlaying = true;
            var nextTrackDuration = await PlayNextTrack();
            CreateTimer();
            _trackTimer.Change(nextTrackDuration, nextTrackDuration);
            _trackTimerAutoEvent.WaitOne();
        }

        private void CreateTimer()
        {
            _trackTimerAutoEvent = new AutoResetEvent(false);
            _trackTimer = new Timer(TimerDone, new AutoResetEvent(false), 0, 0);
        }
        
        private async void TimerDone(object stateInfo)
        {
            _trackTimerAutoEvent.Set();
            
            if (_trackTimer != null)
            {
                var nextTrackDuration = await PlayNextTrack();
                _trackTimer.Change(nextTrackDuration, nextTrackDuration);
                _trackTimerAutoEvent.WaitOne();
            }
        }
        
        public async Task StopPlayingTracks(int afterDelay)
        {
            await Task.Delay(afterDelay);
            _isCurrentlyPlaying = false;
            _playbackQueue.Reset();

            if (_trackTimer != null)
            {
                await _trackTimer.DisposeAsync();
                _trackTimer = null;
            }
            
            await PausePlayerForAll();
        }

        public async Task SkipTrack()
        {
            if (_trackTimer != null)
            {
                await _trackTimer.DisposeAsync();
                _trackTimer = null;
            }
            
            var nextTrackDuration = await PlayNextTrack();
            
            CreateTimer();
            
            _trackTimer.Change(nextTrackDuration, nextTrackDuration);
            _trackTimerAutoEvent.WaitOne();
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
            NotifyObserversPlayingStatus(_isCurrentlyPlaying);
            
            return nextTrack.SongDurationMs;
        }
        
        private async Task PlayTrackForAll(TrackDto track)
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

        private async Task PausePlayerForAll()
        {
            var responseTasks = new List<Task<HttpResponseMessage>>();

            // Iterate through all connected users and collect Tasks for pausing spotify on their devices
            foreach (var keyValuePair in PlaybackListenerManager.ConnectedUsers)
            {
                var pauseTask =
                    _spotifyPlayerService.PausePlayer(keyValuePair.Key, keyValuePair.Value.SpotifyAccessToken, String.Empty);
                
                responseTasks.Add(pauseTask);
            }

            //await all the collected tasks
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