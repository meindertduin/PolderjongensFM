using Pjfm.Domain.Enums;
using Pjfm.WebClient.Services;

namespace Pjfm.Application.Common.Dto
{
    public class PlaybackSettingsDto
    {
        public bool IsPlaying { get; set; }
        public TopTrackTermFilter PlaybackTermFilter { get; set; }
        public PlaybackState PlaybackState { get; set; }
    }
}