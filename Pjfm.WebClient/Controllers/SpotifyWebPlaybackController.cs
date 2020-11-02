using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Identity;
using Pjfm.Domain.Entities;
using Pjfm.WebClient.Services;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/spotiy/playback")]
    public class SpotifyWebPlaybackController : ControllerBase
    {
        private readonly IPlaybackController _playbackController;

        public SpotifyWebPlaybackController(IPlaybackController playbackController)
        {
            _playbackController = playbackController;
        }
        
        [HttpPut("on")]
        public IActionResult TurnOnPlaybackController()
        {
            _playbackController.TurnOn(PlaybackControllerCommands.TrackPlayerOnOff);
            return Accepted();
        }

        [HttpPut("off")]
        public IActionResult TurnOffPlaybackController()
        {
            _playbackController.TurnOff(PlaybackControllerCommands.TrackPlayerOnOff);
            return Accepted();
        }

        [HttpPut("term")]
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

        [HttpPut("reset")]
        public IActionResult ResetPlayer()
        {
            _playbackController.TurnOn(PlaybackControllerCommands.ResetPlaybackCommand);
            return Accepted();
        }
    }
}