using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;
using FluentEmail.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Auth.Querys;
using Pjfm.Application.Identity;

namespace Pjfm.WebClient.Pages.Account
{
    public class Register : PageModel
    {
        [BindProperty] public bool EmailSend { get; set; }
        [BindProperty] public RegisterForm Form { get; set; }
        
        public void OnGet([FromServices] IConfiguration configuration)
        {
            Form = new RegisterForm(){ReturnUrl = configuration["AppUrls:ClientBaseUrl"], Summeries = new List<string>()};
        }
        
        public async Task<IActionResult> OnPost([FromServices] UserManager<ApplicationUser> userManager, [FromServices] IMediator mediator,
            [FromServices] IConfiguration configuration, [FromServices] IFluentEmail fluentEmail)
        {
            Form.Summeries = new List<string>();
            
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var newUser = new ApplicationUser(Form.Email){Email = Form.Email, DisplayName = Form.Username};
            var userCreateRequest = await userManager.CreateAsync(newUser, Form.Password);
            
            if (userCreateRequest.Succeeded)
            {
                var emailConfirmToken = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
                
                await mediator.Send(new LoginCommand()
                {
                    EmailAddress = Form.Email,
                    Password = Form.Password,
                });
                
                var confirmEmailUrl = $"{configuration["AppUrls:ApiBaseUrl"]}" 
                                      + "/account/ConfirmEmail" 
                                      + $"?code={HttpUtility.UrlEncode(emailConfirmToken)}"
                                      + $"&userId={newUser.Id}";
                
                var email =  fluentEmail.To(Form.Email)
                    .Subject("Verifiëren mail-address account pjfm")
                    .Body("Volg de volgende link om uw account te verifiëren: \r\n" + confirmEmailUrl);

                await email.SendAsync();
                
                EmailSend = true;
                return Page();
            }
            
            foreach (var identityError in userCreateRequest.Errors)
            {
                switch (identityError.Code)
                {
                    case "PasswordTooShort":
                        Form.Summeries.Add("Wacthwoord is te kort");
                        break;
                    case "PasswordRequiresNonAlphanumeric":
                        Form.Summeries.Add("Wacthwoord heeft een niet alphanumerisch teken nodig");
                        break;
                    case "PasswordRequiresDigit":
                        Form.Summeries.Add("wachtwoort heeft een cijfer nodig");
                        break;
                    case "PasswordRequiresLower":
                        Form.Summeries.Add("Wacthwoord heeft een normale letter nodig");
                        break;
                    case "PasswordRequiresUpper":
                        Form.Summeries.Add("Wacthwoord heeft een hoofdletter nodig");
                        break;
                    case "DuplicateUserName":
                        Form.Summeries.Add("Email is al in gebruik");
                        break; 
                }
            }
            
            return Page();
        }
    }

    public class RegisterForm
    {
        public string ReturnUrl { get; set; }
        public List<string> Summeries { get; set; }
        
        [Required(ErrorMessage = "veld is verplicht")]
        [MaxLength(50, ErrorMessage = "gebruikersnaam te lang")]
        public string Username { get; set; }

        [Required(ErrorMessage = "veld is verplicht")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Voer een geldig email address in")]
        [MaxLength(200, ErrorMessage = "e-mailaddress te lang")]
        public string Email { get; set; }
        
        [Required (ErrorMessage = "veld is verplicht")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Wachtwoorden zijn niet hetzelfde")]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Required (ErrorMessage = "veld is verplicht")]
        public string ConfirmPassword { get; set; }
    }
}