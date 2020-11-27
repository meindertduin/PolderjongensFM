using System.Collections.Concurrent;
using System.Threading.Tasks;
using Pjfm.Application.Identity;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackListenerManager
    {
        Task AddListener(ApplicationUser user);
        Task<ApplicationUser> RemoveListener(string userId);

        bool TrySetTimedListener(string userId, int minutes);
        bool TryRemoveTimedListener(string userId);
        bool IsUserTimedListener(string userId);
    }
}