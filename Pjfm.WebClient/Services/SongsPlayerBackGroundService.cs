using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace pjfm.Services
{
    public class SongsPlayerBackGroundService : BackgroundService
    {
        public SongsPlayerBackGroundService()
        {
            
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}