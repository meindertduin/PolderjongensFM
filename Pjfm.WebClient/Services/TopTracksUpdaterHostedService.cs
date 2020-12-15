using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pjfm.Application.Spotify.Commands;
using Pjfm.Domain.Interfaces;
using Serilog;

namespace pjfm.Services
{
    public class TopTracksUpdaterHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public TopTracksUpdaterHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(InitializeTopTracksUpdate, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;

            void InitializeTopTracksUpdate(object state)
            {
                Task.Run(CheckForUpdate, cancellationToken);
            }
        }

        private async Task CheckForUpdate()
        {
            Log.Information("Updating top tracks of users");

            using var scope = _serviceProvider.CreateScope();
            var appDbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                
            var users = appDbContext.ApplicationUsers
                .Where(user => String.IsNullOrEmpty(user.SpotifyRefreshToken) == false)
                .ToArray();

            List<Task> updateTasks = new List<Task>();
            
            foreach (var user in users)
            {
                var updateTask = mediator.Send(new UpdateUserTopTracksCommand()
                {
                    User = user,
                });
                
                updateTasks.Add(updateTask);
            }

            await Task.WhenAll(updateTasks);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}