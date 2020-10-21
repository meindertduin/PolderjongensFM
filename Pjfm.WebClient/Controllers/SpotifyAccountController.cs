using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Identity;
using Pjfm.Application.Spotify.Commands;
using Pjfm.Domain.Interfaces;
using Pjfm.Domain.ValueObjects;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/spotify/account")]
    public class SpotifyAccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppDbContext _ctx;

        public SpotifyAccountController(IMediator mediator, 
            IConfiguration configuration, 
            UserManager<ApplicationUser> userManager,
            IAppDbContext ctx)
        {
            _mediator = mediator;
            _configuration = configuration;
            _userManager = userManager;
            _ctx = ctx;
        }
        
        
        [HttpGet("callback")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> FinalizeAuthentication(string state, string code)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var trackedUserProfile = _ctx.ApplicationUsers.FirstOrDefault(a => a.Id == user.Id);

            if (user == null || trackedUserProfile == null)
            {
                Forbid();
            }

            var result = await _mediator.Send(new AccessTokensRequestCommand()
            {
                ClientSecret = _configuration["Spotify:ClientSecret"],
                ClientId = _configuration["Spotify:ClientId"],
                Code = code,
                RedirectUri = _configuration["Spotify:CallbackUrl"],
            });
            
            if (user.SpotifyAuthenticated)
            {
                var updateTopTracksResult = await _mediator.Send(new UpdateUserTopTracksCommand()
                {
                    AccessToken = result.Data.AccessToken,
                    User = user,
                });
                
                if (updateTopTracksResult.Error == false)
                {
                    return Redirect("https://localhost:5001");
                }
            }
            
            var setTopTracksResult = await _mediator.Send(new SetUserTopTracksCommand()
            {
                AccessToken = result.Data.AccessToken,
                User = user,
            });
            
            if (setTopTracksResult.Error == false)
            {
                trackedUserProfile.SpotifyAuthenticated = true;
                await _ctx.SaveChangesAsync(CancellationToken.None);
                return Redirect("https://localhost:5001");
            }

            return BadRequest();
        }
    }
}