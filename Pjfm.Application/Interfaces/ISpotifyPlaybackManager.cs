using System;
using System.Threading.Tasks;
using Pjfm.Application.Identity;

namespace Pjfm.Domain.Interfaces
{
    public interface ISpotifyPlaybackManager : IObservable<bool>
    {
        bool IsCurrentlyPlaying { get; }
        Task<int> PlayNextTrack();
        Task StartPlayingTracks();
        void StopPlayingTracks();
        Task AddListener(ApplicationUser user);
        ApplicationUser RemoveListener(string userId);
    }
}