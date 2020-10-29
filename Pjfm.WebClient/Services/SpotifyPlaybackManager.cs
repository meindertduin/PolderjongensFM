﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
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

        public SpotifyPlaybackManager(IMediator mediator)
        {
            _mediator = mediator;
        }

        public bool IsCurrentlyPlaying { get; private set; }
        
        public async Task<int> PlayNextTrack()
        {
            var nextTrack = _tracksQueue.Dequeue();
            await AddRandomSongToQueue(1);
            return nextTrack.SongDurationMs;
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
        private void PlayTrackOnListenerDevice()
        {
            
        }

        public async Task StartPlayingTracks()
        {
            IsCurrentlyPlaying = true;
            await AddRandomSongToQueue(_tracksQueueLength);
        }

        public void StopPlayingTracks()
        {
            IsCurrentlyPlaying = false;
            _tracksQueue = new Queue<TopTrack>();
            _recentlyPlayed = new List<TopTrack>();
            
            // implement login for stopping playback on all devices
        }

        public void TuneIn()
        {
            
        }
    }
}