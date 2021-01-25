using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Identity;

namespace Pjfm.WebClient.Pages.Account
{
    public class ConfirmResetPassword : PageModel
    {
        [BindProperty] public ResetPasswordForm Form { get; set; }
        
        public void OnGet(string code, string userId)
        {
            Form = new ResetPasswordForm { Code = code, UserId = userId };
        }

        public async Task<IActionResult> OnPost([FromServices] IConfiguration configuration, 
            [FromServices] UserManager<ApplicationUser> userManager)
        {
            if (ModelState.IsValid == false)
            {
                return Forbid();
            }

            var user = await userManager.FindByIdAsync(Form.UserId);
            var result = await userManager.ResetPasswordAsync(user, Form.Code, Form.Password);

            if (result.Succeeded)
            {
                return Redirect(configuration["AppUrls:ClientBaseUrl"]);
            }

            return Forbid();
        }
    }

    public class ResetPasswordForm
    {
        [Required] 
        public string UserId { get; set; }
        [Required]
        public string Code { get; set; }
        
        [Required (ErrorMessage = "veld is verplicht")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Wachtwoorden zijn niet hetzelfde")]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Required (ErrorMessage = "veld is verplicht")]
        public string ConfirmPassword { get; set; }
    }
}