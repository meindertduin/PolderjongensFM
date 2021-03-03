using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.AppContexts.Auth.Commands;

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