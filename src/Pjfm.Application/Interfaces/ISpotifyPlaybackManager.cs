using System;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Domain.Entities;
using Pjfm.WebClient.Services;

namespace Pjfm.Domain.Interfaces
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