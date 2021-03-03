using System;
using System.Threading.Tasks;
using Pjfm.Application.AppContexts.Playback;
using Pjfm.Application.AppContexts.Tracks;

namespace Pjfm.Application.Interfaces
{
    public interface ISpotifyPlaybackManager : IObservable<bool>
    {
        bool IsCurrentlyPlaying { get; protected set; }
        DateTime CurrentTrackStartTime { get;}
        TrackDto CurrentPlayingTrack { get; }
        Task SkipTrack();
        Task<int> PlayNextTrack();
        Task StartPlayingTracks();
        Task ResetPlayingTracks(int afterDelay);
        Task StopPlayback(int afterDelay);
        Task SynchWithCurrentPlayer(string userId, string accessToken, PlaybackDevice playbackDevice);
    }
}