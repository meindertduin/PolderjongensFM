using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Pjfm.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            // services.AddSingleton(c =>
            // {
            //     var config = new PjfmMapperConfiguration(c =>
            //     {
            //         var profiles = Assembly.GetExecutingAssembly().GetTypes()
            //             .Where(t => t.IsSubclassOf(typeof(AutoMapper.Profile)));
            //
            //         foreach (var profile in profiles)
            //         {
            //             c.AddProfile(profile);
            //         }
            //     });
            //
            //     return config.CreateMapper();
            // });

            return services;
        }
    }
}