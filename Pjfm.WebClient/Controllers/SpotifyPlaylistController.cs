using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.Services;
using Pjfm.Domain.Common;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/playlist")]
    public class SpotifyPlaylistController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISpotifyBrowserService _spotifyBrowserService;

        public SpotifyPlaylistController(UserManager<ApplicationUser> userManager, ISpotifyBrowserService spotifyBrowserService)
        {
            _userManager = userManager;
            _spotifyBrowserService = spotifyBrowserService;
        }
        
        public async Task<IActionResult> Get()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var playlistRequestResult = await _spotifyBrowserService.GetUserPlaylists(user.Id, user.SpotifyAccessToken,
                new PlaylistRequestDto()
                {
                    Limit = 50,
                    Offset = 0,
                });

            var content = await playlistRequestResult.Content.ReadAsStringAsync();
            
            return Ok(content);
        }
    }
}