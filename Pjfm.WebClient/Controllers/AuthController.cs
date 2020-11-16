using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Auth.Querys;
using Pjfm.Application.Identity;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("mod")]
        public IActionResult GetModStatus()
        {
            var isMod = HttpContext.User.HasClaim(ApplicationIdentityConstants.Claims.Role,
                ApplicationIdentityConstants.Roles.Mod);

            return Ok(isMod);
        }
        
        [HttpGet("profile")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> GetUserProfile()
        {
            var response = await _mediator.Send(new UserProfileQuery()
            {
                UserClaimPrincipal = HttpContext.User,
            });

            if (response.Error)
            {
                return Forbid();
            }

            return Ok(response);
        }
        
        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var logoutResult = await _mediator.Send(new LogoutCommand()
            {
                LogoutId = logoutId,
            });

            if (logoutResult.Error)
            {
                return BadRequest();
            }

            return Redirect(logoutResult.Data);
        } 
    }
}