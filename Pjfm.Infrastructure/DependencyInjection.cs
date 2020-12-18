using System.Linq;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pjfm.Application.Identity;
using Pjfm.Application.Services;
using Pjfm.Domain.Interfaces;
using Pjfm.Infrastructure.Persistence;
using Pjfm.Infrastructure.Service;

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
            
            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseInMemoryDatabase("DevIdentity");
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
                        options.Password.RequireDigit = true;
                        options.Password.RequiredLength = 8;
                        options.Password.RequireUppercase = true;
                        options.Password.RequireLowercase = true;
                        options.Password.RequireNonAlphanumeric = false;
                    }
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            var identityServiceBuilder = services.AddIdentityServer();
            identityServiceBuilder.AddAspNetIdentity<ApplicationUser>();

            if (webHostEnvironment.IsDevelopment())
            {
                identityServiceBuilder.AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = builder => builder.UseInMemoryDatabase("DevIdentity");
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder => builder.UseInMemoryDatabase("DevIdentity");
                    })
                    .AddInMemoryIdentityResources(ApplicationIdentityConfiguration.GetIdentityResources())
                    .AddInMemoryClients(ApplicationIdentityConfiguration.GetClients())
                    .AddInMemoryApiScopes(ApplicationIdentityConfiguration.GetApiScopes());
                
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
        
        private static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in ApplicationIdentityConfiguration.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in ApplicationIdentityConfiguration.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in ApplicationIdentityConfiguration.GetApiScopes())
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}