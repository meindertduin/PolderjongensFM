using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Auth.Querys;
using Pjfm.Application.Mappings;

namespace Pjfm.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(LogoutCommand).Assembly);
            services.AddSingleton(c =>
            {
                var config = new PjfmMapperConfiguration(c =>
                {
                    var profiles = Assembly.GetExecutingAssembly().GetTypes()
                        .Where(t => t.IsSubclassOf(typeof(AutoMapper.Profile)));

                    foreach (var profile in profiles)
                    {
                        c.AddProfile(profile);
                    }
                });

                return config.CreateMapper();
            });

            return services;
        }
    }
}