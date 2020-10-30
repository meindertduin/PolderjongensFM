using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Interfaces;

namespace pjfm.Services
{
    public class SongsPlayerBackGroundService : BackgroundService, IObserver<bool>
    {
        private readonly ISpotifyPlaybackManager _playbackManager;
        private IDisposable _unsubscriber;
        private bool isPlaying = false;
        public SongsPlayerBackGroundService(ISpotifyPlaybackManager playbackManager)
        {
            _playbackManager = playbackManager;
            _unsubscriber = playbackManager.Subscribe(this);
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (isPlaying)
                {
                    var nextSongDuration = await _playbackManager.PlayNextTrack();
                    await Task.Delay(nextSongDuration);
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }

        public void OnCompleted()
        {
            
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(bool value)
        {
            isPlaying = value;
        }
    }
}