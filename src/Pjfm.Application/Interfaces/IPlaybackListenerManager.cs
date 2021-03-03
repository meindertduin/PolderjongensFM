using System.Threading.Tasks;
using Pjfm.Application.AppContexts.Playback;
using Pjfm.Domain.Entities;

namespace Pjfm.Application.Interfaces
{
    public interface IPlaybackListenerManager
    {
        Task AddListener(ApplicationUser user, PlaybackDevice playbackDevice);
        ApplicationUser RemoveListener(string userId);
        int? GetUserSubscribeTime(string userId);
        bool TrySetTimedListener(string userId, int minutes, string userConnectionId);
        bool TryRemoveTimedListener(string userId);
        bool IsUserTimedListener(string userId);
    }
}