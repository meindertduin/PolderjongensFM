using Microsoft.AspNetCore.Mvc;

namespace Pjfm.Bff.Controllers
{
    public class UserController : Controller
    {
        [Route("logout")]
        public IActionResult Logout()
        {
            return SignOut("cookies", "oidc");
        }
    }
}