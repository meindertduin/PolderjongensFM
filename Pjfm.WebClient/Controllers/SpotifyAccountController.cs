using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Spotify.Commands;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/spotify/account")]
    public class SpotifyAccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public SpotifyAccountController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }
        
        [HttpGet("authenticate")]
        public async Task<IActionResult> AuthenticateSpotify(string state, string code)
        {
            var result = await _mediator.Send(new AccessTokensRequestCommand()
            {
                ClientSecret = _configuration["Spotify:ClientSecret"],
                ClientId = _configuration["Spotify:ClientId"],
                Code = code,
                RedirectUri = "https://localhost:5001/api/spotify/account/authenticate",
            });

            return Ok(result.Data);
        }
    }
}