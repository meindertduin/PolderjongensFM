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

        private readonly IServiceProvider _serviceProvider;
        private readonly ISpotifyPlayerService _spotifyPlayerService;

        private static readonly ConcurrentDictionary<string, ApplicationUser> _connectedUsers 
            = new ConcurrentDictionary<string, ApplicationUser>();

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
            if (_connectedUsers.Count <= 0)
            {
                IsCurrentlyPlaying = false;
                NotifyObserversPlayingStatus(IsCurrentlyPlaying);
            
                _recentlyPlayed = new List<TopTrack>();
            
                // Todo: implement stopping playback on all devices
            }
        }
        
        public async Task<int> PlayNextTrack()
        {
            int nextTrackDuration;
            
            if (_recentlyPlayed.Count > 0)
            {
                var randomTracks = await GetRandomTracks(1);
                
                nextTrackDuration = randomTracks[0].SongDurationMs;

                _recentlyPlayed.AddRange(randomTracks);
                await SpotifyQueueNextTrack(randomTracks[0]);
            }
            else
            {
                var randomTracks = await GetRandomTracks(_initialQueueLength);
                
                nextTrackDuration = randomTracks[0].SongDurationMs;

                _recentlyPlayed.AddRange(randomTracks);
                await InitializePlayer(randomTracks);
            }

            return nextTrackDuration;

        }

        private async Task InitializePlayer(List<TopTrack> tracks)
        {
            var responseTasks = new List<Task<HttpResponseMessage>>();

            string[] uris = new string[tracks.Count];

            for (int i = 0; i < tracks.Count; i++)
            {
                uris[i] = $"spotify:track:{tracks[i].Id}";
            }
            
            foreach (var keyValuePair in _connectedUsers)
            {
                var playTask = _spotifyPlayerService.Play(keyValuePair.Key, keyValuePair.Value.SpotifyAccessToken, String.Empty, 
                    new PlayRequestDto()
                {
                    Uris = uris,
                });
                responseTasks.Add(playTask);
            }
            
            _playerInitTime = DateTime.Now;
            await Task.WhenAll(responseTasks);
        }
        
        
        private async Task SpotifyQueueNextTrack(TopTrack nextTrack)
        {
            _recentlyPlayed.Add(nextTrack);
            
            var responseTasks = new List<Task<HttpResponseMessage>>();

            foreach (var keyValuePair in _connectedUsers)
            {
                var queueTask = _spotifyPlayerService.AddTrackToQueue(keyValuePair.Key,
                    keyValuePair.Value.SpotifyAccessToken, nextTrack.Id);
                
                responseTasks.Add(queueTask);
            }

            await Task.WhenAll(responseTasks);
        }

        private async Task SynchWithCurrentPlayer(string userId, string accessToken)
        {
            var synchedRequestData = GetSynchronisedRequestData();
             await  _spotifyPlayerService.Play(userId, accessToken, String.Empty, synchedRequestData);
        }

        private PlayRequestDto GetSynchronisedRequestData()
        {
            var timePassed = DateTime.Now - _playerInitTime;
            var msPassed = timePassed.TotalMilliseconds;

            double timeIncremented = 0;
            
            var requestInfo = new PlayRequestDto();

            foreach (var track in _recentlyPlayed)
            {
                if (timeIncremented + track.SongDurationMs < msPassed)
                {
                    timeIncremented += track.SongDurationMs;
                }
                else
                {
                    var songMilliseconds = msPassed - timeIncremented;

                    requestInfo.Uris = new[] {$"spotify:track:{track.Id}"};
                    requestInfo.PositionMs = (int) songMilliseconds;
                    break;
                }
            }
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
        public async Task AddListener(ApplicationUser user)
        {
            _connectedUsers[user.Id] = user;
            
            if (IsCurrentlyPlaying == false)
            {
                await StartPlayingTracks();
            }
            else
            {
                await SynchWithCurrentPlayer(user.Id, user.SpotifyAccessToken);
            }
        }

        public async Task<ApplicationUser> RemoveListener(string userId)
        {
            _connectedUsers.TryRemove(userId, out ApplicationUser user);

            if (_connectedUsers.IsEmpty)
            {
                StopPlayingTracks(30_000);
            }
            
            return user;
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