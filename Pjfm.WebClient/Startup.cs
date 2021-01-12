using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pjfm.Application;
using Pjfm.Application.Common;
using Pjfm.Application.Identity;
using Pjfm.Domain.Interfaces;
using pjfm.Hubs;
using Pjfm.Infrastructure;
using Pjfm.Infrastructure.Persistence;
using pjfm.Models;
using pjfm.Services;
using Pjfm.WebClient.Services;
using Serilog;

namespace pjfm
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddInfrastructure(Configuration, WebHostEnvironment);
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Pjfm.Api", Version = "v1"});
            });
            
            services.AddSingleton<ISpotifyPlaybackManager, SpotifyPlaybackManager>();
            services.AddSingleton<IPlaybackListenerManager, PlaybackListenerManager>();
            services.AddSingleton<IPlaybackQueue, PlaybackQueue>();
            services.AddSingleton<IPlaybackEventTransmitter, PlaybackEventTransmitter>();
            
            services.AddTransient<IPlaybackController, PlaybackController>();
            services.AddTransient<IDjHubMessageService, DjHubMessageService>();

            services.AddMediatR(typeof(SpotifyPlaybackManager).Assembly);
            
            services.AddAutoMapper(typeof(MappingProfile), typeof(ViewModelMappingProfile));

            services.AddControllersWithViews();

            services.AddHostedService<TopTracksUpdaterHostedService>();

            services.AddSignalR();
            
            services.AddRazorPages();
            

            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigins",
                    builder =>
                    {
                        builder
                            .AllowCredentials()
                            .WithOrigins(Configuration["AppUrls:ClientBaseUrl"])
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pjfm.Api v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();

                var forwardOptions = new ForwardedHeadersOptions()
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                    RequireHeaderSymmetry = false,
                };
                
                forwardOptions.KnownNetworks.Clear();
                forwardOptions.KnownProxies.Clear();
                
                app.UseForwardedHeaders(forwardOptions);
            }

            app.UseSerilogRequestLogging();
            
            app.UseCors("AllowOrigins");
            
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(10),
            };
            webSocketOptions.AllowedOrigins.Add();

            app.UseWebSockets(webSocketOptions);
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseCookiePolicy(new CookiePolicyOptions()
            {
                MinimumSameSitePolicy = SameSiteMode.None,
                Secure = CookieSecurePolicy.Always,
            });

            app.UseAuthentication();
            
            app.UseIdentityServer();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<RadioHub>("/radio");
                endpoints.MapHub<LiveChatHub>("/livechat");
                endpoints.MapHub<DjHub>("/radio/dj");
                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                
                endpoints.MapRazorPages();
            });
        }

        private static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in ApplicationIdentityConfiguration.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChangesAsync();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in ApplicationIdentityConfiguration.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChangesAsync();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in ApplicationIdentityConfiguration.GetApiScopes())
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChangesAsync();
                }
            }
        }
    }
}
