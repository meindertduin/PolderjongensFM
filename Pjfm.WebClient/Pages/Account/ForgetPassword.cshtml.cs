using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public IActionResult OnPost()
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }
            
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