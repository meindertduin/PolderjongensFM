using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityServer4.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.Spotify.Queries;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Infrastructure.Service
{
    public class SpotifyPlaybackManager : ISpotifyPlaybackManager
    {
        private int _tracksQueueLength = 3;
        
        private TopTrack _currentPlayingTrack;
        private List<TopTrack> _recentlyPlayed = new List<TopTrack>();
        private Queue<TopTrack> _tracksQueue = new Queue<TopTrack>();

        private readonly IMediator _mediator;
        private readonly ISpotifyPlayerService _spotifyPlayerService;

        private static readonly ConcurrentDictionary<string, ApplicationUser> _connectedUsers 
            = new ConcurrentDictionary<string, ApplicationUser>();

        private static List<IObserver<bool>> _observers = new List<IObserver<bool>>();
        
        public SpotifyPlaybackManager(IMediator mediator, ISpotifyPlayerService spotifyPlayerService)
        {
            _mediator = mediator;
            _spotifyPlayerService = spotifyPlayerService;
        }

        public bool IsCurrentlyPlaying { get; private set; }
        
        public async Task StartPlayingTracks()
        {
            IsCurrentlyPlaying = true;
            NotifyObserversPlayingStatus(IsCurrentlyPlaying);
        }

        public void StopPlayingTracks()
        {
            IsCurrentlyPlaying = false;
            NotifyObserversPlayingStatus(IsCurrentlyPlaying);
            
            _tracksQueue = new Queue<TopTrack>();
            _recentlyPlayed = new List<TopTrack>();
            
            // Todo: implement stopping playback on all devices
        }
        
        public async Task<int> PlayNextTrack()
        {
            await AddRandomSongToQueue(_tracksQueueLength);
            var nextTrack = _tracksQueue.Dequeue();
            await AddRandomSongToQueue(1);
            
            if (_recentlyPlayed.Count > 0)
            {
                await AddTrackToListenerQueue();
            }
            else
            {
                await InitializeFirstTrack(nextTrack);
            }
            
            return nextTrack.SongDurationMs;
        }

        private async Task InitializeFirstTrack(TopTrack track)
        {
            var responseTasks = new List<Task<HttpResponseMessage>>();
            
            foreach (var keyValuePair in _connectedUsers)
            {
                var playTask = _spotifyPlayerService.Play(keyValuePair.Key, keyValuePair.Value.SpotifyAccessToken, String.Empty, 
                    new PlayRequestDto()
                {
                    Uris = new [] { $"spotify:track:{track.Id}"},
                });
                responseTasks.Add(playTask);
            }
            
            await Task.WhenAll(responseTasks);
        }

        private async Task AddTrackToListenerQueue()
        {
            
        }

        private async Task AddRandomSongToQueue(int amount)
        {
            var result = await _mediator.Send(new GetRandomTopTrackQuery()
            {
                NotIncludeTracks = _recentlyPlayed,
                RequestedAmount = amount,
            });
            
            if (result.Data.Count > 0)
            {
                foreach (var topTrack in result.Data)
                {
                    _tracksQueue.Enqueue(topTrack);
                }
            }
            else
            {
                _recentlyPlayed = new List<TopTrack>();
                await AddRandomSongToQueue(amount);
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
                // Todo add functionality for tuning in and synchronising with the player
            }
        }

        public ApplicationUser RemoveListener(string userId)
        {
            _connectedUsers.TryRemove(userId, out ApplicationUser user);

            if (_connectedUsers.IsEmpty)
            {
                StopPlayingTracks();    
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