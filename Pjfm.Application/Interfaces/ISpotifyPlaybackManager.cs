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
        Task StopPlayingTracks(int afterDelay);
        Task AddListener(ApplicationUser user);
        Task<ApplicationUser> RemoveListener(string userId);
    }
}