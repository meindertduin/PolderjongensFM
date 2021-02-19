using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Interfaces;
using pjfm.Models;
using pjfm.Services;
using Serilog;

namespace Pjfm.WebClient.Services
{
    public class SpotifyPlaybackManager : ISpotifyPlaybackManager
    {
        private int _fillerQueueLength = 50;
        
        private static readonly List<IObserver<bool>> _observers = new List<IObserver<bool>>();
        
        private readonly ISpotifyPlayerService _spotifyPlayerService;
        private readonly IPlaybackQueue _playbackQueue;
        private readonly IDjHubMessageService _djHubMessageService;

        private Timer _trackTimer;
        private AutoResetEvent _trackTimerAutoEvent;
        private bool _isCurrentlyPlaying;

        public SpotifyPlaybackManager(ISpotifyPlayerService spotifyPlayerService, 
            IPlaybackQueue playbackQueue, IDjHubMessageService djHubMessageService)
        {
            _spotifyPlayerService = spotifyPlayerService;
            _playbackQueue = playbackQueue;
            _djHubMessageService = djHubMessageService;

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
            // turns playback on if its off
            if (_isCurrentlyPlaying == false)
            {
                _isCurrentlyPlaying = true;
                await _playbackQueue.SetUsers();
            
                var nextTrackDuration = await PlayNextTrack();
                
                // sets the timer based on the next track duration
                if (nextTrackDuration > 0)
                {
                    CreateTimer();
                    _trackTimer.Change(nextTrackDuration, nextTrackDuration);
                    _trackTimerAutoEvent.WaitOne();
                }
            }
        }

        public async Task ResetPlayingTracks(int afterDelay)
        {
            await StopPlayback(afterDelay);
            
            _isCurrentlyPlaying = true;
            
            // get next track duration and set timer
            var nextTrackDuration = await PlayNextTrack();
            if (nextTrackDuration > 0)
            {
                CreateTimer();
                _trackTimer.Change(nextTrackDuration, nextTrackDuration);
                _trackTimerAutoEvent.WaitOne();
            }
        }

        private void CreateTimer()
        {
            _trackTimerAutoEvent = new AutoResetEvent(false);
            
            // create a timer and sets TimerDone as eventHandler when timer is done
            _trackTimer = new Timer(TimerDone, new AutoResetEvent(false), 0, 0);
        }
        
        private async void TimerDone(object stateInfo)
        {
            _trackTimerAutoEvent.Set();

            // set a new timer
            if (_trackTimer != null)
            {
                var nextTrackDuration = await PlayNextTrack();
                _trackTimer.Change(nextTrackDuration, nextTrackDuration);
                _trackTimerAutoEvent.WaitOne();
            }
        }
        
        public async Task StopPlayback(int afterDelay)
        {
            await Task.Delay(afterDelay);
            _isCurrentlyPlaying = false;
            
            // reset all tracks in the playbackQueue
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
            
            // reset the timer to the nextTrackDuration
            _trackTimer.Change(nextTrackDuration, nextTrackDuration);
            _trackTimerAutoEvent.WaitOne();
        }
        
        public async Task<int> PlayNextTrack()
        {
            // gets new batch of filler queue tracks if there were no recentlyPlayedTracks
            if (_playbackQueue.RecentlyPlayedCount() <= 0)
            {
                await _playbackQueue.AddToFillerQueue(_fillerQueueLength);
            }
            
            try
            {
                var nextTrack = await _playbackQueue.GetNextQueuedTrack();
                if (nextTrack != null)
                {
                    CurrentPlayingTrack = nextTrack;
                    CurrentTrackStartTime = DateTime.Now;
                    
                    await PlayTrackForAll(nextTrack);
                    NotifyObserversPlayingStatus(_isCurrentlyPlaying);

                    if (nextTrack.SongDurationMs > 0)
                    {
                        return nextTrack.SongDurationMs;
                    }
                }

                throw new InvalidDataException(nameof(nextTrack));
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                await HandlePlaybackErrorShutdown("Playback is afgesloten, kon geen volgend nummer verkrijgen.");
                return 0;
            }
        }

        private async Task HandlePlaybackErrorShutdown(string message)
        {
            // notify dj of error with the playback
            _djHubMessageService.SendMessageToClient(new HubServerMessage()
            {
                Message = message,
                Error = true,
            });
            
            await StopPlayback(0);
        }
        
        private async Task PlayTrackForAll(TrackDto track)
        {
            var responseTasks = new List<Task<HttpResponseMessage>>();

            // iterate through all connected users and make request to spotify to play track on users client
            foreach (var keyValuePair in PlaybackListenerManager.ConnectedUsers)
            {
                var user = keyValuePair.Value.User;
                var playbackDevice = keyValuePair.Value.PlaybackDevice;

                var deviceId = playbackDevice.Id ?? String.Empty;
                
                var playTask = _spotifyPlayerService.Play(keyValuePair.Key, user.SpotifyAccessToken, deviceId,
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
                    _spotifyPlayerService.PausePlayer(keyValuePair.Key, keyValuePair.Value.User.SpotifyAccessToken, String.Empty);
                
                responseTasks.Add(pauseTask);
            }

            //await all the collected tasks
            await Task.WhenAll(responseTasks);
        }
        
        public async Task SynchWithCurrentPlayer(string userId, string accessToken, PlaybackDevice playbackDevice)
        {
            var synchedRequestData = GetSynchronisedRequestData();
            
            // play track on user spotify client
            await _spotifyPlayerService.Play(userId, accessToken, playbackDevice.Id ?? String.Empty, synchedRequestData);
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
            // subscribe to the event which will fire on every new track being played
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