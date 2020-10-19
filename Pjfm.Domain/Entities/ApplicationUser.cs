using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Pjfm.Domain.Entities;

namespace Pjfm.Application.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser(string userName) : base(userName)
        {
            
        }
        
        public ICollection<TopTrack> TopTracks { get; set; }
    }
}