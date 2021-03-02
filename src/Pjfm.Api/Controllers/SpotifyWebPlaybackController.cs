using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Ganss.XSS;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Common;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Common.Dto.Queries;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Users.Queries;
using Pjfm.Application.Services;
using Pjfm.Domain.Enums;
using Pjfm.Domain.Interfaces;
using Pjfm.Domain.ValueObjects;
using Pjfm.Infrastructure.Service;
using Pjfm.WebClient.Services;
using Pjfm.WebClient.Services.FillerQueueState;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/playback")]
    public class SpotifyWebPlaybackController : ControllerBase
    {
        private readonly IPlaybackController _playbackController;
        private readonly ISpotifyBrowserService _spotifyBrowserService;
        private readonly IPlaybackInfoTransmitter _infoTransmitter;
        private readonly IMediator _mediator;
        private readonly IPlaybackListenerManager _playbackListenerManager;
        private readonly ISpotifyPlayerService _spotifyPlayerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SpotifyWebPlaybackController(IPlaybackController playbackController,
            ISpotifyBrowserService spotifyBrowserService,
            UserManager<ApplicationUser> userManager,
            IPlaybackInfoTransmitter infoTransmitter,
            IMediator mediator,
            IPlaybackListenerManager playbackListenerManager,
            ISpotifyPlayerService spotifyPlayerService)
        {
            _playbackController = playbackController;
            _spotifyBrowserService = spotifyBrowserService;
            _infoTransmitter = infoTransmitter;
            _mediator = mediator;
            _playbackListenerManager = playbackListenerManager;
            _spotifyPlayerService = spotifyPlayerService;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets the spotify playback devices of the user that queries
        /// </summary>
        [HttpGet("devices")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> GetPlaybackDevices()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var result = await _mediator.Send(new GetPlaybackDevicesQuery()
            {
                UserId = user.Id,
            });

            return Ok(result.Data);
        }
        
        /// <summary>
        /// Turns the playback on
        /// </summary>
        [HttpPut("mod/on")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult TurnOnPlaybackController()
        {
            _playbackController.TurnOn(PlaybackControllerCommands.TrackPlayerOnOff);
            return Accepted();
        }

        /// <summary>
        /// Turn the playback off
        /// </summary>
        [HttpPut("mod/off")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult TurnOffPlaybackController()
        {
            _playbackController.TurnOff(PlaybackControllerCommands.TrackPlayerOnOff);
            return Accepted();
        }

        /// <summary>
        /// force a user out of the saved timed listeners and pause users spotify with spotify api
        /// </summary>
        [HttpPut("forceStop")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public async Task<IActionResult> ForceStopPlaybackUser()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var removeResult = _playbackListenerManager.TryRemoveTimedListener(user.Id);
            
            if (removeResult)
            {
                await _spotifyPlayerService.PausePlayer(user.Id, user.SpotifyAccessToken, String.Empty);
                return Accepted();
            }

            return NoContent();
        }

        /// <summary>
        /// resets the playback
        /// </summary>
        [HttpPut("mod/reset")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult ResetPlayer([FromQuery] int maxRequestAmount, [FromQuery] TopTrackTermFilter term)
        {
            if (maxRequestAmount >= 0)
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
                _playbackController.SetMaxRequestsPerUserAmount(maxRequestAmount);
                
                _playbackController.TurnOn(PlaybackControllerCommands.ResetPlaybackCommand);
                return Accepted();
            }
            return BadRequest();
        }

        [HttpPut("mod/dequeueTrack")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult DequeueTrack([FromQuery] string trackId)
        {
            _playbackController.DequeueTrack(trackId);
            return Accepted();
        }
        
        /// <summary>
        /// Search topTracks with parameters
        /// </summary>
        /// <returns>Response object with topTracks</returns>
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

        /// <summary>
        /// request a track with track id to be played in the playback.
        /// This method will handle a request from a mod differently than
        /// from a user/
        /// </summary>
        [HttpPut("request/{trackId}")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> UserTrackRequest(string trackId, [FromQuery] string message)
        {
            var track = await GetRequestedTrack(trackId);
            if (track != null)
            {
                
                if (message != null)
                {
                    HtmlSanitizer sanitizer = new HtmlSanitizer();
                    track.Message = sanitizer.Sanitize(message.WithMaxLength(100));
                }
                
                var user = await _userManager.GetUserAsync(HttpContext.User);

                // add track to queue
                var response = _playbackController.AddSecondaryTrack(track,
                    ApplicationUserSerializer.SerializeToDto(user));
                
                if (response.Error)
                {
                    return Conflict(response);
                }
                
                // publish queue state to all users connected to radio hub
                _infoTransmitter.PublishUpdatePlaybackInfoEvents();

                return Accepted(response);
            }
            
            return BadRequest();
        }

        [HttpPut("mod/request/{trackId}")]
        public async Task<IActionResult> ModTrackRequest(string trackId, [FromQuery] string message)
        {
            var track = await GetRequestedTrack(trackId);
            if (track != null)
            {
                if (message != null)
                {
                    HtmlSanitizer sanitizer = new HtmlSanitizer();
                    track.Message = sanitizer.Sanitize(message.WithMaxLength(100));
                }
                
                var response = _playbackController.AddPriorityTrack(track);
                
                if (response.Error)
                {
                    return Conflict(response);
                }
                
                // publish queue state to all users connected to radio hub
                _infoTransmitter.PublishUpdatePlaybackInfoEvents();

                return Accepted(response);
            }

            return BadRequest();
        }

        private async Task<TrackDto> GetRequestedTrack(string trackId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var trackResponse = await _spotifyBrowserService.GetTrackInfo(user.Id, user.SpotifyAccessToken, trackId);

            if (trackResponse.IsSuccessStatusCode)
            {
                var requestedTrack = SerializeTrackOfResponse(await trackResponse.Content.ReadAsStringAsync());
                return requestedTrack;
            }

            return null;
        }
        
        /// <summary>
        /// Skips a track in the playback
        /// </summary>
        [HttpPut("mod/skip")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult SkipCurrentTrack()
        {
            _playbackController.TurnOn(PlaybackControllerCommands.TrackSkip);
            return Accepted();
        }

        /// <summary>
        /// Include a user's topTracks into the playback, changes will require a reset to take effect
        /// </summary>
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

        /// <summary>
        /// Exclude a user's topTracks from the playback, changes will require a reset to take effect
        /// </summary>
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

        /// <summary>
        /// Gets all included users in the playback
        /// </summary>
        [HttpGet("mod/include")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult GetIncludedUsers()
        {
            var result = _playbackController.GetIncludedUsers();
            return Ok(result);
        }
        
        /// <summary>
        /// Sets the playback state of the playback in which it will play, these changes
        /// Will be immediately updated and wont require a refresh
        /// </summary>
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
                case PlaybackState.RoundRobinPlaybackState:
                    _playbackController.TurnOn(PlaybackControllerCommands.SetRoundRobinPlaybackState);
                    break;
                default:
                    return BadRequest();
            }
            return Accepted();
        }

        /// <summary>
        /// Sets the fillerQueueState in the playbackQueue Class
        /// </summary>
        /// <param name="fillerQueueType"></param>
        /// <returns>Status 200 on accept</returns>
        [HttpPut("mod/fillerQueueState")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult SetFillerQueueState([FromQuery] FillerQueueType fillerQueueType)
        {
            _playbackController.SetFillerQueueState(fillerQueueType);
            return Ok();
        }

        /// <summary>
        /// Sets the browserQueueSettings in the fillerQueueSettings class that is hold
        /// by the PlaybackQueue Class
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>Returns 200 on accept</returns>
        [HttpPost("mod/browserQueueSettings")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public IActionResult SetBrowserQueueSettings([FromBody] BrowserQueueSettings settings)
        {
            _playbackController.SetBrowserQueueSettings(settings);
            return Ok();
        }

        /// <summary>
        /// Gets the genreSeeds of the spotify api
        /// </summary>
        /// <returns></returns>
        [HttpGet("mod/spotifyGenres")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public async Task<IActionResult> GetSpotifyGenres()
        {
            var response = await _spotifyBrowserService.GetSpotifyGenres();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return Ok(json);
            }

            return StatusCode((int) response.StatusCode);
        }
        
        private TrackDto SerializeTrackOfResponse(string trackResponseContent)
        {
            var trackSerializer = new SpotifyTrackSerializer();
            var trackDto = trackSerializer.ConvertSingle(trackResponseContent);

            return trackDto;
        }
    }
}