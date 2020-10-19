using IdentityServer4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pjfm.Application.Identity;
using Pjfm.Infrastructure.Persistance;

namespace Pjfm.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            services.AddDbContext<ApplicationIdentityDbContext>(config =>
            {
                config.UseInMemoryDatabase("DevIdentity");
            });
            
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
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
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            var identityServiceBuilder = services.AddIdentityServer();

            if (webHostEnvironment.IsDevelopment())
            {
                identityServiceBuilder.AddDeveloperSigningCredential();
            }

            services.AddLocalApiAuthentication();
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy(ApplicationIdentityConstants.Policies.User, builder =>
                {
                    builder.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
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