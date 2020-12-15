using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Auth.Querys;
using Pjfm.Application.Identity;
using Pjfm.WebClient.Services;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPlaybackListenerManager _playbackListenerManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IMediator mediator, IPlaybackListenerManager playbackListenerManager, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _playbackListenerManager = playbackListenerManager;
            _userManager = userManager;
        }

        [HttpGet("mod")]
        public IActionResult GetModStatus()
        {
            var user = HttpContext.User.Claims;
            
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
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var logoutResult = await _mediator.Send(new LogoutCommand()
            {
                LogoutId = logoutId,
            });

            if (logoutResult.Error)
            {
                return BadRequest();
            }

            if (user.Id != null)
            {
                _playbackListenerManager.TryRemoveTimedListener(user.Id);
            }
            
            return Redirect(logoutResult.Data);
        } 
    }
}