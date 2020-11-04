using System;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Auth.Querys;
using Pjfm.Application.Services;
using Pjfm.Application.Spotify.Queries;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(LogoutCommand).Assembly);

            services.AddHttpClient<ISpotifyHttpClientService, SpotifyHttpClientService>();
            services.AddTransient<ISpotifyPlayerService, SpotifyPlayerService>();
            services.AddTransient<ISpotifyBrowserService, SpotifyBrowserService>();
            
            return services;
        }
    }
}