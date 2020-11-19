using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pjfm.Application.Identity;

namespace pjfm
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                
                var mod = new ApplicationUser("mod"){Email = "mod@mail.com"};
                mod.Member = true;
                userManager.CreateAsync(mod, "password").GetAwaiter().GetResult();
                userManager.AddClaimAsync(mod,
                    new Claim(ApplicationIdentityConstants.Claims.Role, 
                        ApplicationIdentityConstants.Roles.Mod));
            }
                
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
