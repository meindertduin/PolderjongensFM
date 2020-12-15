using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pjfm.Application.Auth.Querys;
using Pjfm.Application.Spotify.Commands;

namespace Pjfm.WebClient.Pages.Account
{
    public class Login : PageModel
    {
        [BindProperty] public LoginForm Form { get; set; }
        
        public void OnGet(string returnUrl)
        {
            Form = new LoginForm() {ReturnUrl = returnUrl};
        }
        
        public async Task<IActionResult> OnPost([FromServices] IMediator mediator)
        {
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
                return Page();
            }
            
            return Redirect(Form.ReturnUrl);

        }
    }
    
    public class LoginForm
    {
        public string ReturnUrl { get; set; }
        [Required(ErrorMessage = "Verplicht veld")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Voer een geldig email in")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Verplicht veld")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}