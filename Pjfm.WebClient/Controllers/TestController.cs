using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Identity;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet("user")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public string GetUserMessage()
        {
            return "user";
        }

        [HttpGet("mod")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public string GetModMessage()
        {
            return "mod";
        }
    }
}