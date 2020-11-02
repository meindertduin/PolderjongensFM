using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.Spotify.Queries;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class SpotifyPlaybackManager : ISpotifyPlaybackManager
    {
        private int _initialQueueLength = 3;

        private DateTime _playerInitTime;
        
        private TopTrack _currentPlayingTrack;
        private List<TopTrack> _recentlyPlayed = new List<TopTrack>();

        private int _nextTrackIndex = 0;

        private readonly IServiceProvider _serviceProvider;
        private readonly ISpotifyPlayerService _spotifyPlayerService;
        
        private static readonly List<IObserver<bool>> _observers = new List<IObserver<bool>>();
        
        private TrackTimer _timer;

        public SpotifyPlaybackManager(IServiceProvider serviceProvider, ISpotifyPlayerService spotifyPlayerService)
        {
            _serviceProvider = serviceProvider;
            _spotifyPlayerService = spotifyPlayerService;
            
            _timer = new TrackTimer(this);
        }

        public bool IsCurrentlyPlaying { get; private set; }
        
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
            
            _recentlyPlayed = new List<TopTrack>();
            
            // Todo: implement stopping playback on all devices
        }
        
        public async Task<int> PlayNextTrack()
        {
            int nextTrackDuration;
            
            if (_recentlyPlayed.Count > 0)
            {
                var randomTracks = await GetRandomTracks(1);
                
                nextTrackDuration = _recentlyPlayed[_nextTrackIndex].SongDurationMs;
                _nextTrackIndex += 1;
                
                _recentlyPlayed.AddRange(randomTracks);
                await SpotifyQueueNextTrack(randomTracks[0]);
            }
            else
            {
                var randomTracks = await GetRandomTracks(_initialQueueLength);
                
                nextTrackDuration = randomTracks[0].SongDurationMs;
                _nextTrackIndex = 1;

                _recentlyPlayed.AddRange(randomTracks);
                await InitializePlayer(randomTracks);
            }

            return nextTrackDuration;
        }

        private async Task InitializePlayer(List<TopTrack> tracks)
        {
            var responseTasks = new List<Task<HttpResponseMessage>>();
            
            foreach (var keyValuePair in PlaybackListenerManager.ConnectedUsers)
            {
                var playTask = _spotifyPlayerService.Play(keyValuePair.Key, keyValuePair.Value.SpotifyAccessToken, String.Empty, 
                    new PlayRequestDto()
                {
                    Uris = new []{ $"spotify:track:{tracks[0].Id}"}
                });
                responseTasks.Add(playTask);
            }
            
            _playerInitTime = DateTime.Now;
            
            await Task.WhenAll(responseTasks);

            for (int i = 1; i < tracks.Count; i++)
            {
                await SpotifyQueueNextTrack(tracks[i]);
            }
        }
        private async Task SpotifyQueueNextTrack(TopTrack nextTrack)
        {
            var responseTasks = new List<Task<HttpResponseMessage>>();

            foreach (var keyValuePair in PlaybackListenerManager.ConnectedUsers)
            {
                var queueTask = _spotifyPlayerService.AddTrackToQueue(keyValuePair.Key,
                    keyValuePair.Value.SpotifyAccessToken, nextTrack.Id);
                
                responseTasks.Add(queueTask);
            }

            await Task.WhenAll(responseTasks);
        }

        public async Task SynchWithCurrentPlayer(string userId, string accessToken)
        {
            var synchedRequestData = GetSynchronisedRequestData(out int songIndex);
            await _spotifyPlayerService.Play(userId, accessToken, String.Empty, synchedRequestData);
            for (int i = songIndex + 1; i < _recentlyPlayed.Count; i++)
            {
                await _spotifyPlayerService.AddTrackToQueue(userId, accessToken, _recentlyPlayed[i].Id);
            }
        }

        private PlayRequestDto GetSynchronisedRequestData(out int songIndex)
        {
            var timePassed = DateTime.Now - _playerInitTime;
            var msPassed = timePassed.TotalMilliseconds;
            int index = 0;

            double timeIncremented = 0;
            
            var requestInfo = new PlayRequestDto();

            for (int i = 0; i < _recentlyPlayed.Count; i++)
            {
                if (timeIncremented + _recentlyPlayed[i].SongDurationMs < msPassed)
                {
                    timeIncremented += _recentlyPlayed[i].SongDurationMs;
                }
                else
                {
                    var songMilliseconds = msPassed - timeIncremented;

                    requestInfo.Uris = new[] {$"spotify:track:{_recentlyPlayed[i].Id}"};
                    requestInfo.PositionMs = (int) songMilliseconds;
                    index = 1;
                    break;
                }
            }

            songIndex = index;
            return requestInfo;
        }

        private async Task<List<TopTrack>> GetRandomTracks(int amount)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tracks = new List<TopTrack>();
                    
                var _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await _mediator.Send(new GetRandomTopTrackQuery()
                {
                    NotIncludeTracks = _recentlyPlayed,
                    RequestedAmount = amount,
                });
                    
                if (result.Data.Count > 0)
                {
                    foreach (var topTrack in result.Data)
                    {
                        tracks.Add(topTrack);
                    }

                    return tracks;
                }

                _recentlyPlayed = new List<TopTrack>();
                return await GetRandomTracks(amount);
            }
            
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