using System.Collections.Concurrent;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;

namespace Pjfm.WebClient.Services
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