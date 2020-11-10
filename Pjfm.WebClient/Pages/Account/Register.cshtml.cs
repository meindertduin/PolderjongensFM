using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pjfm.Application.Auth.Querys;
using Pjfm.Application.Identity;

namespace Pjfm.WebClient.Pages.Account
{
    public class Register : PageModel
    {
        [BindProperty] public RegisterForm Form { get; set; }
        
        public void OnGet(string returnUrl)
        {
            Form = new RegisterForm(){ReturnUrl = returnUrl};
        }
        
        public async Task<IActionResult> OnPost([FromServices] UserManager<ApplicationUser> userManager, 
            [FromServices] SignInManager<ApplicationUser> signInManager, [FromServices] IMediator mediator)
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var newUser = new ApplicationUser(Form.Username){Email = Form.Email};
            var userCreateRequest = await userManager.CreateAsync(newUser, Form.Password);

            if (userCreateRequest.Succeeded)
            {
                var loginResult = await mediator.Send(new LoginCommand()
                {
                    Username = Form.Username,
                    Password = Form.Password,
                });

                if (loginResult.Error)
                {
                    return Page();
                }
            
                return Redirect(Form.ReturnUrl);
            }
            return Page();
        }
    }

    public class RegisterForm
    {
        public string ReturnUrl { get; set; }
        [Required(ErrorMessage = "Gebruikersnaam is verplicht")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email addres is verplicht")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Voer een geldig email address in")]
        public string Email { get; set; }
        
        [Required (ErrorMessage = "Watchwoord is verplicht")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Wachtwoorden zijn niet hetzelfde")]
        public string ConfirmPassword { get; set; }

    }
}