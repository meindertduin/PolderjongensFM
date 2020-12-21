using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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
        [BindProperty] public RegisterForm Form { get; set; }
        
        public void OnGet([FromServices] IConfiguration configuration)
        {
            Form = new RegisterForm(){ReturnUrl = configuration["AppUrls:ClientBaseUrl"], Summeries = new List<string>()};
        }
        
        public async Task<IActionResult> OnPost([FromServices] UserManager<ApplicationUser> userManager, 
            [FromServices] SignInManager<ApplicationUser> signInManager, [FromServices] IMediator mediator,
            [FromServices] IConfiguration configuration)
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var newUser = new ApplicationUser(Form.Email){Email = Form.Email, DisplayName = Form.Username};
            var userCreateRequest = await userManager.CreateAsync(newUser, Form.Password);

            if (userCreateRequest.Succeeded)
            {
                var loginResult = await mediator.Send(new LoginCommand()
                {
                    EmailAddress = Form.Email,
                    Password = Form.Password,
                });
                
                var authorizationUrl = "https://accounts.spotify.com/authorize" + 
                    "?client_id=ebc49acde46148eda6128d944c067b5d" + 
                    "&response_type=code" +
                    $@"&redirect_uri={configuration["AppUrls:ApiBase"]}/api/spotify/account/callback" + 
                    "&scope=user-top-read user-read-private streaming user-read-playback-state playlist-read-private playlist-read-collaborative";

                return Redirect(authorizationUrl);
            }

            Form.Summeries ??= new List<string>();
            
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
        public string Username { get; set; }

        [Required(ErrorMessage = "veld is verplicht")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Voer een geldig email address in")]
        public string Email { get; set; }
        
        [Required (ErrorMessage = "veld is verplicht")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Required (ErrorMessage = "veld is verplicht")]
        [Compare("Password", ErrorMessage = "Wachtwoorden zijn niet hetzelfde")]
        public string ConfirmPassword { get; set; }
    }
}