using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pjfm.Application.Identity;
using Pjfm.Domain.Entities;

namespace Pjfm.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<TopTrack> TopTracks { get; set; }
        

    }
}