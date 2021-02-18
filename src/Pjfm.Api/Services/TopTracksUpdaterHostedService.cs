using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pjfm.Application.Spotify.Commands;
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
            _timer = new Timer(InitializeTopTracksUpdate, null, TimeSpan.Zero, TimeSpan.FromHours(12));
            return Task.CompletedTask;

            // local function to be able to wrap the Task.Run function
            void InitializeTopTracksUpdate(object state)
            {
                Task.Run(CheckForUpdate, cancellationToken);
            }
        }

        private async Task CheckForUpdate()
        {
            Log.Information("Updating top tracks of users");

            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            // refresh all users TopTracks that are spotify authenticated
            await mediator.Send(new UpdateAllUsersTopTracks());
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