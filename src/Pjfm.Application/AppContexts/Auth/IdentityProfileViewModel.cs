using Pjfm.Application.AppContexts.Auth.Querys;

namespace Pjfm.Application.AppContexts.Auth
{
    public class IdentityProfileViewModel
    {
        public UserProfileViewModel UserProfile { get; set; }
        public bool IsMod { get; set; }
        public bool IsSpotifyAuthenticated { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}