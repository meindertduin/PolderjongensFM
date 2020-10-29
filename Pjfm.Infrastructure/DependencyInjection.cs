﻿using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pjfm.Application.Identity;
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

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookie")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";

                    options.Authority = "https://localhost:5001";
                    options.RequireHttpsMetadata = false;
                    options.ClientSecret = "secret";
                    options.ClientId = "mvc";

                    options.ResponseType = "code id_token";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("offline_access");
                    options.Scope.Add("api1");
                    options.ClaimActions.MapJsonKey("website", "website");
                });

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