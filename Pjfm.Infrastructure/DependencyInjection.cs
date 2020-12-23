using System;
using System.Linq;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pjfm.Application.Identity;
using Pjfm.Application.Services;
using Pjfm.Domain.Interfaces;
using Pjfm.Infrastructure.Persistence;
using Pjfm.Infrastructure.Service;
using pjfm.Services;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Pjfm.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            services.AddHttpClient<ISpotifyHttpClientService, SpotifyHttpClientService>();
            
            services.AddTransient<ISpotifyPlayerService, SpotifyPlayerService>();
            services.AddTransient<ISpotifyBrowserService, SpotifyBrowserService>();
            
            services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>());
            services.AddTransient<IRetrieveStrategy, SpotifyTopTracksRetrieveStrategy>();

            var connectionString = configuration["ConnectionStrings:ApplicationDb"];
            
            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseMySql(connectionString, 
                    builder =>
                    {
                        builder.MigrationsAssembly("Pjfm.WebClient");
                        builder.ServerVersion(new ServerVersion(new Version(10, 3, 25), ServerType.MariaDb));
                    });
            });
            
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    if (webHostEnvironment.IsDevelopment())
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequiredLength = 6;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                    }
                    else
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequiredLength = 8;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                    }
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            var identityServiceBuilder = services.AddIdentityServer();
            identityServiceBuilder.AddAspNetIdentity<ApplicationUser>();

            identityServiceBuilder.AddProfileService<ProfileService>();
            
            identityServiceBuilder.AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseMySql(connectionString, builder =>
                    {
                        builder.MigrationsAssembly("Pjfm.WebClient");
                        builder.ServerVersion(new ServerVersion(new Version(10, 3, 25), ServerType.MariaDb));
                    });
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseMySql(connectionString, builder =>
                    {
                        builder.MigrationsAssembly("Pjfm.WebClient");
                        builder.ServerVersion(new ServerVersion(new Version(10, 3, 25), ServerType.MariaDb));
                    });
                })
                .AddInMemoryIdentityResources(ApplicationIdentityConfiguration.GetIdentityResources())
                .AddInMemoryClients(ApplicationIdentityConfiguration.GetClients())
                .AddInMemoryApiScopes(ApplicationIdentityConfiguration.GetApiScopes());
                
            
            // Todo: add production signing credentials 
            
            identityServiceBuilder
                .AddDeveloperSigningCredential();
            
            services.AddLocalApiAuthentication();

            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Account/Login";
                config.LogoutPath = "/api/auth/logout";
            });
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy(ApplicationIdentityConstants.Policies.User, builder =>
                {
                    builder.RequireAuthenticatedUser();
                });
                options.AddPolicy(ApplicationIdentityConstants.Policies.Mod, builder =>
                {
                    var defaultPolicy = options.DefaultPolicy;
                    builder.Combine(defaultPolicy);
                    builder.RequireClaim(ApplicationIdentityConstants.Claims.Role,
                        ApplicationIdentityConstants.Roles.Mod);
                });
            });
            
            return services;
        }
    }
}