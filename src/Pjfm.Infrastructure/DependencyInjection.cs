using System;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pjfm.Application.Identity;
using Pjfm.Application.Interfaces;
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
            
            services.AddTransient<IAppDbContext>(provider => provider.GetService<AppDbContext>());
            services.AddTransient<IAppDbContextFactory, DatabaseFactory>();

            var connectionString = configuration["ConnectionStrings:ApplicationDb"];
            
            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseMySql(connectionString, 
                    builder =>
                    {
                        builder.MigrationsAssembly("Pjfm.Api");
                        builder.ServerVersion(new ServerVersion(new Version(10, 3, 25), ServerType.MariaDb));
                    });
            }, ServiceLifetime.Transient);
            
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

            if (webHostEnvironment.IsProduction())
            {
                identityServiceBuilder.AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = builder => builder.UseMySql(connectionString, builder =>
                        {
                            builder.MigrationsAssembly("Pjfm.Api");
                            builder.ServerVersion(new ServerVersion(new Version(10, 3, 25), ServerType.MariaDb));
                        });
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder => builder.UseMySql(connectionString, builder =>
                        {
                            builder.MigrationsAssembly("Pjfm.Api");
                            builder.ServerVersion(new ServerVersion(new Version(10, 3, 25), ServerType.MariaDb));
                        });
                    });
            }
            else
            {
                identityServiceBuilder
                    .AddInMemoryIdentityResources(ApplicationIdentityConfiguration.GetIdentityResources())
                    .AddInMemoryClients(ApplicationIdentityConfiguration.GetClients())
                    .AddInMemoryApiScopes(ApplicationIdentityConfiguration.GetApiScopes());
            }

            if (webHostEnvironment.IsProduction())
            {
                identityServiceBuilder.AddSigningCredential(
                    new X509Certificate2(configuration["Crypt:Cert"], configuration["Crypt:Password"]));
            }
            else
            {
                identityServiceBuilder.AddDeveloperSigningCredential();
            }
            
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