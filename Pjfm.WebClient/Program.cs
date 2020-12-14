using System;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Identity;
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
                //.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var testUser = new ApplicationUser("test"){ Email = "test@mail.com"};
                testUser.Member = true;
                userManager.CreateAsync(testUser, "password").GetAwaiter().GetResult();
                
                var jeremyUser = new ApplicationUser("Jeremy"){ Email = "jeremymulder1@gmail.com"};
                jeremyUser.Member = true;
                userManager.CreateAsync(jeremyUser, "password").GetAwaiter().GetResult();
                
                var jordiUser = new ApplicationUser("Jordi"){ Email = "jordimulder7@gmail.com"};
                jordiUser.Member = true;
                userManager.CreateAsync(jordiUser, "password").GetAwaiter().GetResult();
                
                var mod = new ApplicationUser("mod"){Email = "mod@mail.com"};
                mod.Member = true;
                userManager.CreateAsync(mod, "password").GetAwaiter().GetResult();
                userManager.AddClaimAsync(mod,
                    new Claim(ApplicationIdentityConstants.Claims.Role, 
                        ApplicationIdentityConstants.Roles.Mod));
            }

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
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
