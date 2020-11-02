using System;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pjfm.Application;
using Pjfm.Application.Common;
using Pjfm.Domain.Interfaces;
using pjfm.Hubs;
using Pjfm.Infrastructure;
using Pjfm.Infrastructure.Service;
using Pjfm.WebClient.Services;
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
            
            services.AddSingleton<ISpotifyPlaybackManager, SpotifyPlaybackManager>();
            services.AddSingleton<IPlaybackListenerManager, PlaybackListenerManager>();

            services.AddMediatR(typeof(SpotifyPlaybackManager).Assembly);
            
            services.AddControllersWithViews();

            services.AddSignalR();
            
            services.AddRazorPages();

            services.AddHttpClient();
            
            services.AddHttpClient(ApplicationConstants.HttpClientNames.SpotifyApiClient, client =>
            {
                client.BaseAddress = new Uri("https://api.spotify.com");
            });
            
            services.AddHttpClient(ApplicationConstants.HttpClientNames.ApiClient, client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                            .AllowCredentials()
                            .WithOrigins("https://localhost:5001")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            
            
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            
            app.UseCors("AllowAllOrigins");
            
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseIdentityServer();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<RadioHub>("/radio");
                endpoints.MapHub<LiveChatHub>("/livechat");
                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                if (env.IsDevelopment())
                {
                    endpoints.MapToVueCliProxy(
                        @"{*path}",
                        new SpaOptions { SourcePath = "ClientApp" },
                        npmScript: "serve",
                        regex: "Compiled successfully");
                }

                endpoints.MapRazorPages();
                
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
            });
        }
    }
}
