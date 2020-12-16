using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.Services;
using Pjfm.Application.Test.Queries;

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
        
        [HttpGet]
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

        [HttpGet]
        [Route("tracks")]
        // TODO: schoonmaken
        public async Task<IActionResult> GetPlaylistTopTracks([FromQuery] string playlistId,[FromQuery] int limit = 100 ,[FromQuery] int offset = 0)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var definition = new { next = "" };
            
            var firstPlaylistTracksResult = await _spotifyBrowserService.GetPlaylistTracks(user.Id, user.SpotifyAccessToken,
                new PlaylistTracksRequestDto()
                {
                    PlaylistId = playlistId,
                    Limit = limit,
                    Offset = offset,
                });

            
            var content = JsonConvert.DeserializeAnonymousType(await firstPlaylistTracksResult.Content.ReadAsStringAsync(), definition);
            string contentString = null;
            
            if(content.next != null){ 
                contentString = "\"results\": [" + await firstPlaylistTracksResult.Content.ReadAsStringAsync() + ", ";
            }
            else
            {
                contentString = "\"results\": [" + await firstPlaylistTracksResult.Content.ReadAsStringAsync();
            }

            while (content.next != null)
            {
                var recursivePlaylistTracksResult = await _spotifyBrowserService.CustomRequest(user.Id, user.SpotifyAccessToken, new Uri(content.next));
                content = JsonConvert.DeserializeAnonymousType(await recursivePlaylistTracksResult.Content.ReadAsStringAsync(), definition);
                contentString += await recursivePlaylistTracksResult.Content.ReadAsStringAsync();
                if (content.next != null)
                {
                    contentString += ", ";
                }
            }
            
            return Ok("{" + contentString + "]}");
        }
        
        
        [HttpGet("top-tracks")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> GetUserTopTracks([FromQuery] string term = "short_term",[FromQuery] int limit = 50 ,[FromQuery] int offset = 0)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var topTracksResult = await _spotifyBrowserService.GetTopTracks(user.Id, user.SpotifyAccessToken,
                new TopTracksRequestDto()
                {
                    Term = term,
                    Limit = limit,
                    Offset = offset,
                });

            var content = await topTracksResult.Content.ReadAsStringAsync();
            
            return Ok(content);
        }
    }
}