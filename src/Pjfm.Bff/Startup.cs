using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pjfm.Bff.Internal;

namespace Pjfm.Bff
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureBff(services);
            ConfigureAuthentication(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            app.UseMiddleware<StrictSameSiteExternalAuthenticationMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.Map("/api", config => RunApiProxy(config, $"{Configuration.GetValue<string>("BackendUrl")}/api"));

            if (env.IsDevelopment())
            {
                // Output errors caused by the UseSpa middleware to the browser
                // Most probably this will be errors indicating the devServer has not been started
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception spaException)
                    {
                        await context.Response.WriteAsync(spaException.Message);
                    }
                });
            }

            if (env.IsDevelopment())
            {
                app.MapWhen(p => p.Request.Path.StartsWithSegments("/sockjs-node"),
                    config =>
                    {
                        config.UseSpa(spa => { spa.UseProxyToSpaDevelopmentServer("http://localhost:8080"); });
                    });
            }

            // Serve the Vue app
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                }
                else
                {
                    spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                    {
                        OnPrepareResponse = DoNotCache
                    };
                }
            });
        }

        private static void DoNotCache(StaticFileResponseContext context)
        {
            context.Context.Request.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            context.Context.Response.Headers.Add("Pragma", "no-cache");
            context.Context.Request.Headers.Add("Expires", "-1");
        }
    }
}