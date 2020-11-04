using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.Services;
using Pjfm.Domain.Entities;
using Pjfm.Infrastructure.Service;
using Pjfm.WebClient.Services;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/playback")]
    public class SpotifyWebPlaybackController : ControllerBase
    {
        private readonly IPlaybackController _playbackController;
        private readonly ISpotifyBrowserService _spotifyBrowserService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SpotifyWebPlaybackController(IPlaybackController playbackController,
            ISpotifyBrowserService spotifyBrowserService, UserManager<ApplicationUser> userManager)
        {
            _playbackController = playbackController;
            _spotifyBrowserService = spotifyBrowserService;
            _userManager = userManager;
        }
        
        [HttpPut("mod/on")]
        public IActionResult TurnOnPlaybackController()
        {
            _playbackController.TurnOn(PlaybackControllerCommands.TrackPlayerOnOff);
            return Accepted();
        }

        [HttpPut("mod/off")]
        public IActionResult TurnOffPlaybackController()
        {
            _playbackController.TurnOff(PlaybackControllerCommands.TrackPlayerOnOff);
            return Accepted();
        }

        [HttpPut("mod/term")]
        public IActionResult AllTermFilter([FromQuery] TopTrackTermFilter term)
        {
            switch (term)
            {
                case TopTrackTermFilter.ShortTerm:
                    _playbackController.TurnOn(PlaybackControllerCommands.ShortTermFilterMode);
                    break;
                case TopTrackTermFilter.MediumTerm:
                    _playbackController.TurnOn(PlaybackControllerCommands.MediumTermFilterMode);
                    break;
                case TopTrackTermFilter.LongTerm:
                    _playbackController.TurnOn(PlaybackControllerCommands.LongTermFilterMode);
                    break;
                case TopTrackTermFilter.ShortMediumTerm:
                    _playbackController.TurnOn(PlaybackControllerCommands.ShortMediumTermFilterMode);
                    break;
                case TopTrackTermFilter.MediumLongTerm:
                    _playbackController.TurnOn(PlaybackControllerCommands.MediumLongTermFilterMode);
                    break;
                case TopTrackTermFilter.AllTerms:
                    _playbackController.TurnOn(PlaybackControllerCommands.AllTermFilterMode);
                    break;
                default:
                    return BadRequest();
            }

            return Accepted();
        }

        [HttpPut("mod/reset")]
        public IActionResult ResetPlayer()
        {
            _playbackController.TurnOn(PlaybackControllerCommands.ResetPlaybackCommand);
            return Accepted();
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchTrack([FromBody] SearchRequestDto searchRequest)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var result = await _spotifyBrowserService.Search(user.Id, user.SpotifyAccessToken, searchRequest);
            var trackSerializer = new SpotifyTrackSerializer();
            var tracks = trackSerializer.ConvertObject(await result.Content.ReadAsStringAsync());

            return Ok(tracks);
        }

        [HttpPut("mod/skip")]
        public IActionResult SkipCurrentTrack()
        {
            _playbackController.TurnOn(PlaybackControllerCommands.TrackSkip);
            return Accepted();
        }
    }
}