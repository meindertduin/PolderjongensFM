using System;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pjfm.Application.Identity;
using Pjfm.Domain.Interfaces;
using Pjfm.WebClient.Services;
using Serilog;
using Serilog.Events;

namespace pjfm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            
            var host = CreateWebHostBuilder(args).Build();

            try
            {
                Log.Information("Starting web host");
                host.Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(((context, builder) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())                    
                    {
                        builder.AddUserSecrets<Program>();
                    }
                }))
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
