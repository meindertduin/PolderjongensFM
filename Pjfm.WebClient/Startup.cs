using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pjfm.Application;
using Pjfm.Application.Common;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.Services;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Interfaces;
using pjfm.Hubs;
using Pjfm.Infrastructure;
using Pjfm.Infrastructure.Persistence;
using Pjfm.Infrastructure.Service;
using pjfm.Models;
using pjfm.Services;
using Pjfm.WebClient.Services;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Serilog;
using VueCliMiddleware;

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

            // HttpClient configuration
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError() // HttpRequestException, 5XX and 408
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

            var invalidOperationException = Policy
                .Handle<InvalidOperationException>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                });
            
            
            services.AddHttpClient<ISpotifyHttpClientService, SpotifyHttpClientService>(o => 
                    o.BaseAddress = new Uri(Configuration["ApiEndpoints:SpotifyEndpoint"]))
                .AddPolicyHandler(retryPolicy);
            
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                            .AllowCredentials()
                            .WithOrigins("https://localhost:8085")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);
            
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
            }

            app.UseSerilogRequestLogging();
            
            app.UseCors("AllowAllOrigins");
            
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
