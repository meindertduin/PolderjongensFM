using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Auth.Querys;

namespace Pjfm.WebClient.Pages.Account
{
    public class Login : PageModel
    {
        [BindProperty] public LoginForm Form { get; set; }
        
        public void OnGet(string returnUrl)
        {
            Form = new LoginForm() {ReturnUrl = returnUrl, Summeries = new List<string>()};
        }
        
        public async Task<IActionResult> OnPost([FromServices] IMediator mediator, [FromServices] IConfiguration configuration)
        {
            Form.Summeries = new List<string>();

            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var loginResult = await mediator.Send(new LoginCommand()
            {
                EmailAddress = Form.EmailAddress,
                Password = Form.Password,
            });

            if (loginResult.Error)
            {
                Form.Summeries.Add("Email of wachtwoord is onjuist");
                return Page();
            }

            if (Form.ReturnUrl != null)
            {
                return Redirect(Form.ReturnUrl);
            }
            
            return Redirect(configuration["AppUrls:ClientBaseUrl"]);

        }
    }
    
    public class LoginForm
    {
        public string ReturnUrl { get; set; }
        public List<string> Summeries { get; set; }

        [Required(ErrorMessage = "Verplicht veld")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Voer een geldig email in")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Verplicht veld")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}