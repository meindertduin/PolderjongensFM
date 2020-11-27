using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;

namespace Pjfm.Application.Services
{
    public class TimedListenerManagerBackgroundService : BackgroundService
    {
        private const int CheckAfterMillisecondsAmount = 60_000;
        
        private static ConcurrentDictionary<string, TimedListenerDto> _timedListeners = new ConcurrentDictionary<string, TimedListenerDto>();
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // checks every user 
                foreach (var timedListener in _timedListeners)
                {
                    
                }

                await Task.Delay(CheckAfterMillisecondsAmount, stoppingToken);
            }
        }
    }
}