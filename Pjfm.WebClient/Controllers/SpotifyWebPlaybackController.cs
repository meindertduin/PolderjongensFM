using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Users.Queries;
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
        private readonly IPlaybackEventTransmitter _eventTransmitter;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public SpotifyWebPlaybackController(IPlaybackController playbackController,
            ISpotifyBrowserService spotifyBrowserService,
            UserManager<ApplicationUser> userManager,
            IPlaybackEventTransmitter eventTransmitter,
            IMediator mediator)
        {
            _playbackController = playbackController;
            _spotifyBrowserService = spotifyBrowserService;
            _eventTransmitter = eventTransmitter;
            _mediator = mediator;
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

            if (result.IsSuccessStatusCode)
            {
                var trackSerializer = new SpotifyTrackSerializer();
                var tracks = trackSerializer.ConvertMultiple(await result.Content.ReadAsStringAsync());
                return Ok(tracks);
            }

            return StatusCode((int) result.StatusCode);
        }

        [HttpPut("request/{trackId}")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> UserRequestTrack(string trackId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var isMod = HttpContext.User.HasClaim(ApplicationIdentityConstants.Claims.Role,
                ApplicationIdentityConstants.Roles.Mod);

            var trackResponse = await _spotifyBrowserService.GetTrackInfo(user.Id, user.SpotifyAccessToken, trackId);

            if (trackResponse.IsSuccessStatusCode)
            {
                var requestedTrack = await SerializeTrackOfResponse(await trackResponse.Content.ReadAsStringAsync());

                if (requestedTrack == null)
                {
                    return BadRequest();
                }

                var response = MakeRequestWithTrackDto(isMod, requestedTrack, user);

                if (response.Error)
                {
                    return Conflict(response);
                }

                _eventTransmitter.PublishUpdatePlaybackInfoEvents();

                return Accepted(response);
            }

            return StatusCode((int) trackResponse.StatusCode);
        }

        private Response<bool> MakeRequestWithTrackDto(bool isMod, TrackDto requestedTrack, ApplicationUser user)
        {
            Response<bool> response;

            if (isMod)
            {
                response = _playbackController.AddPriorityTrack(requestedTrack);
            }
            else
            {
                response = _playbackController.AddSecondaryTrack(requestedTrack,
                    ApplicationUserSerializer.SerializeToDto(user));
            }

            return response;
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
        public async Task<IActionResult> IncludeUsers(ApplicationUserDto user)
        {
            var userResult = await _mediator.Send(new GetUserProfileByIdQuery()
            {
                Id = user.Id,
            });

            if (userResult.Data != null)
            {
                _playbackController.AddIncludedUser(userResult.Data);
                return Accepted();
            }

            return NotFound();
        }

        [HttpPost("mod/exclude")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public async Task<IActionResult> RemoveIncludedUser(ApplicationUserDto user)
        {
            var userResult = await _mediator.Send(new GetUserProfileByIdQuery()
            {
                Id = user.Id,
            });

            if (userResult.Data != null)
            {
                var removeSucceeded = _playbackController.TryRemoveIncludedUser(userResult.Data);
                if (removeSucceeded)
                {
                    return Accepted();
                }

                return StatusCode(500);
            }

            return NotFound();
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
        
        private async Task<TrackDto> SerializeTrackOfResponse(string trackResponseContent)
        {
            var trackSerializer = new SpotifyTrackSerializer();
            var trackDto = trackSerializer.ConvertSingle(trackResponseContent);

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