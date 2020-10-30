using System;
using System.Threading;
using System.Threading.Tasks;
using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class TrackTimerService : ITrackTimerService
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private IDisposable _unsubscriber;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public TrackTimerService(ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
            _unsubscriber = _spotifyPlaybackManager.Subscribe(this);
        }
        
        private async Task StartPlaying()
        {
            while (true)
            {
                var nextTrackLength = await _spotifyPlaybackManager.PlayNextTrack();
                await Task.Delay(nextTrackLength);
            }
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
            if (value)
            {
                Task.Run(StartPlaying, _cts.Token);
            }
            else
            {
                _cts.CancelAfter(0);
            }
        }
    }
}