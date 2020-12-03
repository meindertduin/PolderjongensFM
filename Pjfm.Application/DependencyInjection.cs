using System;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Auth.Querys;
using Pjfm.Application.Common;
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
           

            return services;
        }
    }
}