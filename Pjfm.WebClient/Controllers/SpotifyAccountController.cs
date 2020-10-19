using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Identity;
using Pjfm.Application.Spotify.Commands;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/spotify/account")]
    public class SpotifyAccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public SpotifyAccountController(IMediator mediator, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _configuration = configuration;
            _userManager = userManager;
        }
        
        [HttpGet("authenticate")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> AuthenticateSpotify(string state, string code)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var result = await _mediator.Send(new AccessTokensRequestCommand()
            {
                ClientSecret = _configuration["Spotify:ClientSecret"],
                ClientId = _configuration["Spotify:ClientId"],
                Code = code,
                RedirectUri = _configuration["Spotify:AuthenticateUri"],
            });

            var updateResult = await _mediator.Send(new UpdateUserTopTracksCommand()
            {
                AccessToken = result.Data.AccessToken,
                User = user,
            });
            
            return Ok(result.Data);
        }
    }
}