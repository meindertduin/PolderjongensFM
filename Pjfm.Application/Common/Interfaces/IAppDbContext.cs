using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pjfm.Application.Identity;
using Pjfm.Domain.Entities;

namespace Pjfm.Domain.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<TopTrack> TopTracks { get; set; }
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}