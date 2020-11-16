﻿using System;
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
                .HasForeignKey(t => t.ApplicationUserId);

            builder.Entity<TopTrack>()
                .Property(t => t.Artists)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            builder.Entity<TopTrack>()
                .HasKey(o => new {o.Id, o.ApplicationUserId, o.Term});
        }
    }
}