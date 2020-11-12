using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.Services;
using Pjfm.Domain.Entities;
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
            var tracks = trackSerializer.ConvertMultiple(await result.Content.ReadAsStringAsync());

            return Ok(tracks);
        }

        [HttpPut("request/{trackId}")]
        public async Task<IActionResult> UserRequestTrack(string trackId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            try
            {
                var requestedTrack = await GetTrackOfId(trackId, user);
                var response = _playbackController.AddSecondaryTrack(requestedTrack, user.Id);
                
                if (response.Error)
                {
                    return Conflict(response);
                }

                return Accepted(response);
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
        
        [HttpPut("mod/request/{trackId}")]
        public async Task<IActionResult> ModRequestTrack(string trackId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                var requestedTrack = await GetTrackOfId(trackId, user);
                var response = _playbackController.AddPriorityTrack(requestedTrack);

                if (response.Error)
                {
                    return Conflict(response);
                }

                return Accepted(response);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut("mod/skip")]
        public IActionResult SkipCurrentTrack()
        {
            _playbackController.TurnOn(PlaybackControllerCommands.TrackSkip);
            return Accepted();
        }

        [HttpPost("mod/include")]
        public IActionResult IncludeUsers([FromBody] List<ApplicationUserDto> users)
        {
            _playbackController.SetUsersInclusionList(users);
            return Accepted();
        }
        
        [HttpPut("mod/setPlaybackState")]
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

        [HttpGet("mod/getPlaybackSettings")]
        public IActionResult GetPlaybackSettings()
        {
            var settings = _playbackController.GetPlaybackSettings();
            return Ok(settings);
        }
        
    }
}