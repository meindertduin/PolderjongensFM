using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.Services;

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
        
        public async Task<IActionResult> Get([FromQuery] int limit = 50 ,[FromQuery] int offset = 0)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var playlistRequestResult = await _spotifyBrowserService.GetUserPlaylists(user.Id, user.SpotifyAccessToken,
                new PlaylistRequestDto()
                {
                    Limit = limit,
                    Offset = offset,
                });

            var content = await playlistRequestResult.Content.ReadAsStringAsync();
            
            return Ok(content);
        }

        [Route("tracks")]
        public async Task<IActionResult> GetPlaylistTopTracks([FromQuery] string playlistId,[FromQuery] int limit = 100 ,[FromQuery] int offset = 0)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var playlistTracksResult = await _spotifyBrowserService.GetPlaylistTracks(user.Id, user.SpotifyAccessToken,
                new PlaylistTracksRequestDto()
                {
                    PlaylistId = playlistId,
                    Limit = limit,
                    Offset = offset,
                });

            var content = await playlistTracksResult.Content.ReadAsStringAsync();
            
            return Ok(content);
        }
    }
}