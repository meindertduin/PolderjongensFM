using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pjfm.Domain.Entities;

namespace Pjfm.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<TopTrack> TopTracks { get; set; }
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        
        DbSet<LiveChatMessage> LiveChatMessages { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}