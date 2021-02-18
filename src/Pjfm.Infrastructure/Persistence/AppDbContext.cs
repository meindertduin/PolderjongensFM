using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pjfm.Application.Identity;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<TopTrack> TopTracks { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<LiveChatMessage> LiveChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TopTrack>()
                .HasOne(t => t.ApplicationUser)
                .WithMany(a => a.TopTracks)
                .HasForeignKey(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TopTrack>()
                .Property(t => t.Artists)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .HasMaxLength(200);

            builder.Entity<ApplicationUser>().Property(x => x.Id).HasMaxLength(100);
            builder.Entity<ApplicationUser>().Property(x => x.Email).HasMaxLength(200);
            builder.Entity<ApplicationUser>().Property(x => x.UserName).HasMaxLength(100);
            builder.Entity<ApplicationUser>().Property(x => x.NormalizedUserName).HasMaxLength(100);
            builder.Entity<ApplicationUser>().Property(x => x.PhoneNumber).HasMaxLength(50);
        }
    }
}