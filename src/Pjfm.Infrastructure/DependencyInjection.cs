using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pjfm.Application.Identity;
using Pjfm.Application.Interfaces;
using Pjfm.Application.Services;
using Pjfm.Domain.Interfaces;
using Pjfm.Infrastructure.Persistence;
using Pjfm.Infrastructure.Service;
using pjfm.Services;
using Serilog;

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
                config.UseSqlServer(new SqlConnection(connectionString), builder =>
                {
                    builder.EnableRetryOnFailure();
                    builder.MigrationsAssembly("Pjfm.Api");
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
                        options.ConfigureDbContext = builder => builder.UseSqlServer(new SqlConnection(connectionString), builder =>
                        {
                            builder.EnableRetryOnFailure();
                            builder.MigrationsAssembly("Pjfm.Api");
                        });
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder => builder.UseSqlServer(new SqlConnection(connectionString), builder =>
                        {
                            builder.MigrationsAssembly("Pjfm.Api");
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
            
            identityServiceBuilder.AddDeveloperSigningCredential();
            
            // TODO: this is temporary commented out
            // if (webHostEnvironment.IsProduction())
            // {
            //     identityServiceBuilder.AddSigningCredential(
            //         new X509Certificate2(configuration["Crypt:Cert"], configuration["Crypt:Password"]));
            // }
            // else
            // {
            //     identityServiceBuilder.AddDeveloperSigningCredential();
            // }
            
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