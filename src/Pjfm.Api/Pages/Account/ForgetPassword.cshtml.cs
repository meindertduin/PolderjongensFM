using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Identity;

namespace Pjfm.WebClient.Pages.Account
{
    public class ForgetPassword : PageModel
    {
        [ViewData] public bool EmailSend { get; set; }
        [BindProperty] public ForgetPasswordForm Form { get; set; }
        
        public void OnGet()
        {
            EmailSend = false;
            Form = new ForgetPasswordForm() { Summeries = new List<string>() };
        }

        public async Task<IActionResult> OnPost([FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] IFluentEmail fluentEmail, [FromServices] IConfiguration configuration)
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var user = await userManager.FindByEmailAsync(Form.EmailAddress);
            
            if (user == null)
            {
                return Page();
            }

            var code = await userManager.GeneratePasswordResetTokenAsync(user);

            var confirmResetUrl = $"{configuration["AppUrls:ApiBaseUrl"]}" 
                                  + "/account/ConfirmResetPassword" 
                                  + $"?code={HttpUtility.UrlEncode(code)}"
                                  + $"&userId={user.Id}";
            
            var email =  fluentEmail.To(Form.EmailAddress)
                .Subject("Opnieuw instellen wachtwoord pjfm")
                .Body("Volg de volgende link om een nieuw wachtwoord in te stellen: \r\n" + confirmResetUrl);

            await email.SendAsync();
            
            EmailSend = true;
            return Page();
        }
    }

    public class ForgetPasswordForm
    {
        public List<string> Summeries { get; set; }
        
        [Required(ErrorMessage = "Verplicht veld")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Voer een geldig email in")]
        public string EmailAddress { get; set; }
    }
}