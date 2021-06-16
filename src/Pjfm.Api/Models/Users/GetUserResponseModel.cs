using System.Collections.Generic;
using Pjfm.Domain.Enums;

namespace pjfm.Models.Users
{
    public class GetUserResponseModel
    {
        public string Username { get; set; } = null!;
        public IEnumerable<UserRole> Roles { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public bool SpotifyAuthenticated { get; set; }
    }
}