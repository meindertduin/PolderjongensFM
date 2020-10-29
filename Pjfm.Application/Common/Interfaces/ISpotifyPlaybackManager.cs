using System.Threading.Tasks;
using Pjfm.Domain.Entities;

namespace Pjfm.Domain.Interfaces
{
    public interface ISpotifyPlaybackManager
    {
        bool IsCurrentlyPlaying { get; }
        Task<int> PlayNextTrack();
        Task StartPlayingTracks();
        void StopPlayingTracks();
        void TuneIn();
    }
}