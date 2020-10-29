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
    public class SongsPlayerBackGroundService : BackgroundService
    {
        private readonly ISpotifyPlaybackManager _playbackManager;
        public SongsPlayerBackGroundService(ISpotifyPlaybackManager playbackManager)
        {
            _playbackManager = playbackManager;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_playbackManager.IsCurrentlyPlaying)
                {
                    var nextSongDuration = await _playbackManager.PlayNextTrack();
                    await Task.Delay(nextSongDuration);
                }

                await Task.Delay(1000);
            }
        }
    }
}