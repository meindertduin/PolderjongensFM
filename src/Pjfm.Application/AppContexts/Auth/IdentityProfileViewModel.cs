using Pjfm.Application.Auth.Querys;

namespace pjfm.Models
{
    public class IdentityProfileViewModel
    {
        public UserProfileViewModel UserProfile { get; set; }
        public bool IsMod { get; set; }
        public bool IsSpotifyAuthenticated { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}