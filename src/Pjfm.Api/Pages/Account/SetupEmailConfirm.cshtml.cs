using System.Threading.Tasks;
using System.Web;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Identity;

namespace Pjfm.WebClient.Pages.Account
{
    [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
    public class SetupEmailConfirm : PageModel
    {
        [BindProperty] public string Message { get; set; }
        
        public async Task<IActionResult> OnGet([FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] IConfiguration configuration, [FromServices] IFluentEmail fluentEmail)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return NotFound();
            }
            
            if (user.EmailConfirmed)
            {
                Message = "E-Mailaddress van gebruiker is al geverifieerd";
                return Page();
            }

            var emailConfirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmEmailUrl = $"{configuration["AppUrls:ApiBaseUrl"]}" 
                                  + "/account/ConfirmEmail" 
                                  + $"?code={HttpUtility.UrlEncode(emailConfirmToken)}"
                                  + $"&userId={user.Id}";
                
            var email =  fluentEmail.To(user.Email)
                .Subject("Verifiëren mail-address account pjfm")
                .Body("Volg de volgende link om uw account te verifiëren: \r\n" + confirmEmailUrl);

            await email.SendAsync();

            Message =
                "Een email is gestuurd naar het door uw account geregistreerde mail-address met een link voor het verifiëren van uw account";
            
            return Page();
        }
    }
}