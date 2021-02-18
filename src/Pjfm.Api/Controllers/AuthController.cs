using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Auth.Querys;
using Pjfm.Application.Identity;
using Pjfm.Application.Services;
using Pjfm.Domain.Interfaces;
using Pjfm.WebClient.Services;
using Serilog;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPlaybackListenerManager _playbackListenerManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISpotifyPlayerService _spotifyPlayerService;

        public AuthController(IMediator mediator, IPlaybackListenerManager playbackListenerManager, 
            UserManager<ApplicationUser> userManager, ISpotifyPlayerService spotifyPlayerService)
        {
            _mediator = mediator;
            _playbackListenerManager = playbackListenerManager;
            _userManager = userManager;
            _spotifyPlayerService = spotifyPlayerService;
        }

        /// <summary>
        /// returns a boolean value if the user has the mod claim or not
        /// </summary>
        [HttpGet("mod")]
        public IActionResult GetModStatus()
        {
            var isMod = HttpContext.User.HasClaim(ApplicationIdentityConstants.Claims.Role,
                ApplicationIdentityConstants.Roles.Mod);

            return Ok(isMod);
        }
        
        /// <summary>
        /// query's and returns the user profile identity profile
        /// </summary>
        [HttpGet("profile")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> GetUserProfile()
        {
            var response = await _mediator.Send(new IdentityProfileQuery()
            {
                UserClaimPrincipal = HttpContext.User,
            });

            if (response.Error)
            {
                return Forbid();
            }

            return Ok(response);
        }
        
        /// <summary>
        /// handles the oidc logout
        /// </summary>
        /// <param name="logoutId">a logout id provided by the client</param>
        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            // handle the logout for the user with userManager
            var logoutResult = await _mediator.Send(new LogoutCommand()
            {
                LogoutId = logoutId,
            });

            if (logoutResult.Error)
            {
                return BadRequest();
            }

            if (user != null)
            {
                // if user is still a timed listener try to remove it
                var removed = _playbackListenerManager.TryRemoveTimedListener(user.Id);
                if (removed)
                {
                    try
                    {
                        await _spotifyPlayerService.PausePlayer(user.Id, user.SpotifyAccessToken);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }
                }
            }
            
            return Redirect(logoutResult.Data);
        } 
    }
}