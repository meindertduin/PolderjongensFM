using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.Services;
using Pjfm.Domain.Enums;
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
            ISpotifyBrowserService spotifyBrowserService, 
            UserManager<ApplicationUser> userManager)
        {
            _playbackController = playbackController;
            _spotifyBrowserService = spotifyBrowserService;
            _userManager = userManager;
        }
        
        [HttpPut("mod/on")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult TurnOnPlaybackController()
        {
            _playbackController.TurnOn(PlaybackControllerCommands.TrackPlayerOnOff);
            return Accepted();
        }

        [HttpPut("mod/off")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult TurnOffPlaybackController()
        {
            _playbackController.TurnOff(PlaybackControllerCommands.TrackPlayerOnOff);
            return Accepted();
        }

        [HttpPut("mod/setTerm")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult AllTermFilter([FromQuery] TopTrackTermFilter term)
        {
            switch (term)
            {
                case TopTrackTermFilter.ShortTerm:
                    _playbackController.TurnOn(PlaybackControllerCommands.ShortTermFilterMode);
                    break;
                case TopTrackTermFilter.ShortMediumTerm:
                    _playbackController.TurnOn(PlaybackControllerCommands.ShortMediumTermFilterMode);
                    break;
                case TopTrackTermFilter.MediumTerm:
                    _playbackController.TurnOn(PlaybackControllerCommands.MediumTermFilterMode);
                    break;
                case TopTrackTermFilter.MediumLongTerm:
                    _playbackController.TurnOn(PlaybackControllerCommands.MediumLongTermFilterMode);
                    break;
                case TopTrackTermFilter.LongTerm:
                    _playbackController.TurnOn(PlaybackControllerCommands.LongTermFilterMode);
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
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult ResetPlayer()
        {
            _playbackController.TurnOn(PlaybackControllerCommands.ResetPlaybackCommand);
            return Accepted();
        }

        [HttpPost("search")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> SearchTrack([FromBody] SearchRequestDto searchRequest)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var result = await _spotifyBrowserService.Search(user.Id, user.SpotifyAccessToken, searchRequest);
            var trackSerializer = new SpotifyTrackSerializer();
            var tracks = trackSerializer.ConvertMultiple(await result.Content.ReadAsStringAsync());

            return Ok(tracks);
        }

        [HttpPut("request/{trackId}")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> UserRequestTrack(string trackId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var isMod = HttpContext.User.HasClaim(ApplicationIdentityConstants.Claims.Role, ApplicationIdentityConstants.Roles.Mod);

            var requestedTrack = await GetTrackOfId(trackId, user);

            if (requestedTrack == null)
            {
                return BadRequest();
            }
            
            Response<bool> response;
            
            if (isMod)
            {
                response = _playbackController.AddPriorityTrack(requestedTrack);
            }
            else
            {
                response = _playbackController.AddSecondaryTrack(requestedTrack, ApplicationUserSerializer.SerializeToDto(user));
            }

            if (response.Error)
            {
                return Conflict(response);
            }

            return Accepted(response);
        }
        

        [HttpPut("mod/skip")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult SkipCurrentTrack()
        {
            _playbackController.TurnOn(PlaybackControllerCommands.TrackSkip);
            return Accepted();
        }

        [HttpPost("mod/include")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult IncludeUsers(ApplicationUserDto user)
        {
            _playbackController.AddIncludedUser(user);
            return Accepted();
        }

        [HttpPost("mod/exclude")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult RemoveIncludedUser(ApplicationUserDto user)
        {
            _playbackController.RemoveIncludedUser(user);
            return Accepted();
        }

        [HttpGet("mod/include")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult GetIncludedUsers()
        {
            var result = _playbackController.GetIncludedUsers();
            return Ok(result);
        }
        
        [HttpPut("mod/setPlaybackState")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult SetPlaybackState([FromQuery] PlaybackState playbackState)
        {
            switch (playbackState)
            {
                case PlaybackState.DefaultPlaybackState:
                    _playbackController.TurnOn(PlaybackControllerCommands.SetDefaultPlaybackState);
                    break;
                case PlaybackState.RequestPlaybackState:
                    _playbackController.TurnOn(PlaybackControllerCommands.SetUserRequestPlaybackState);
                    break;
                case PlaybackState.RandomRequestPlaybackState:
                    _playbackController.TurnOn(PlaybackControllerCommands.SetRandomRequestPlaybackState);
                    break;
                default:
                    return BadRequest();
            }
            return Accepted();
        }
        
        private async Task<TrackDto> GetTrackOfId(string trackId, ApplicationUser user)
        {

            var trackResponse = await _spotifyBrowserService.GetTrackInfo(user.Id, user.SpotifyAccessToken, trackId);
            var trackSerializer = new SpotifyTrackSerializer();
            var trackDto = trackSerializer.ConvertSingle(await trackResponse.Content.ReadAsStringAsync());

            return trackDto;
        }

        [HttpGet("mod/playbackSettings")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult GetPlaybackSettings()
        {
            var settings = _playbackController.GetPlaybackSettings();
            return Ok(settings);
        }
        
    }
}