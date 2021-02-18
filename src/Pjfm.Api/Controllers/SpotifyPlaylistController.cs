using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Common.Dto.Queries;
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
        private readonly IMediator _mediator;

        public SpotifyPlaylistController(UserManager<ApplicationUser> userManager, ISpotifyBrowserService spotifyBrowserService, IMediator mediator)
        {
            _userManager = userManager;
            _spotifyBrowserService = spotifyBrowserService;
            _mediator = mediator;
        }
        
        /// <summary>
        /// Get's all the spotify playlist's of the user from the spotify api and returns them
        /// </summary>
        /// <param name="limit">limit of amount of playlists to get</param>
        /// <param name="offset">offset of playlists of users</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the tracks of a playlist
        /// </summary>
        /// <param name="playlistId">The playlist id to get the tracks for</param>
        /// <param name="limit">limit amount of tracks of playlist</param>
        /// <param name="offset">offset of the amount of tracks to be hauled</param>
        [HttpGet]
        [Route("tracks")]
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
            string contentString;
            
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
        
        /// <summary>
        /// Gets the user topTracks
        /// </summary>
        /// <param name="term">The term of what topTracks to retrieve</param>
        /// <param name="limit">the max amount of tracks to get</param>
        /// <param name="offset">the offset topTracks</param>
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