using System;
using System.Threading.Tasks;
using Pjfm.Application.Identity;
using Pjfm.WebClient.Services;

namespace Pjfm.Domain.Interfaces
{
    public interface ISpotifyPlaybackManager : IObservable<bool>
    {
        bool IsCurrentlyPlaying { get; }
        Task ResetPlayer(int afterDelay);
        Task<int> PlayNextTrack();
        Task StartPlayingTracks();
        Task StopPlayingTracks(int afterDelay);
        Task SynchWithCurrentPlayer(string userId, string accessToken);
    }
}