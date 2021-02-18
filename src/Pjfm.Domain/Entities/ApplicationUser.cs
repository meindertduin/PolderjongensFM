using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Pjfm.Domain.Entities;

namespace Pjfm.Application.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser(string userName) : base(userName)
        {
            DisplayName = userName;
        }
        
        public ICollection<TopTrack> TopTracks { get; set; }
        public bool Member { get; set; }
        [MaxLength(50)]
        public string DisplayName { get; set; }
        public bool SpotifyAuthenticated { get; set; }
        public string SpotifyRefreshToken { get; set; }
        public string SpotifyAccessToken { get; set; }
    }
}