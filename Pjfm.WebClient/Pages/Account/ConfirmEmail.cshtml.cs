using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Identity;

namespace Pjfm.WebClient.Pages.Account
{
    public class ConfirmEmail : PageModel
    {
        [BindProperty] public bool ConfirmEmailSucceeded { get; set; }
        
        public async Task<IActionResult> OnGet(string code, string userId, [FromServices] UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Page();
            }

            var confirmResult = await userManager.ConfirmEmailAsync(user, code);

            if (confirmResult.Succeeded)
            {
                user.EmailConfirmed = true;
                ConfirmEmailSucceeded = true;
            }

            return Page();
        }

        public IActionResult OnPost([FromServices] IConfiguration configuration)
        {
            return Redirect(configuration["AppUrls:ClientBaseUrl"]);
        }
    }
}