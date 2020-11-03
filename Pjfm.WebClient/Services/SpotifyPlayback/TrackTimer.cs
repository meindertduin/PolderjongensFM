using System;
using System.Threading;
using System.Threading.Tasks;
using Pjfm.Domain.Interfaces;
using Pjfm.Infrastructure.Service;

namespace Pjfm.WebClient.Services
{
    public class TrackTimer : IObserver<bool>
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private IDisposable _unsubscriber;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        
        private object _timerSetLock = new object();

        private bool _isPlaying = false;
        
        public TrackTimer(SpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
            _unsubscriber = _spotifyPlaybackManager.Subscribe(this);
        }
        
        private async Task StartPlaying(CancellationToken token)
        {
            _isPlaying = true;
            
            while (token.IsCancellationRequested == false)
            {
                var nextTrackLength = await _spotifyPlaybackManager.PlayNextTrack();
                await Task.Delay(nextTrackLength, token);
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
            lock (_timerSetLock)
            {
                if (value && _isPlaying == false)
                {
                    _cts.Cancel();
                    _cts = new CancellationTokenSource();
                    Task.Run(() => StartPlaying(_cts.Token));
                }
                else if (value == false && _isPlaying)
                {
                    _cts.Cancel();
                    _isPlaying = false;
                }
            }
        }
    }
}