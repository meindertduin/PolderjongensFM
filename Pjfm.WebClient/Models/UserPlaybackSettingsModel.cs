using Pjfm.Domain.Enums;

namespace pjfm.Models
{
    public class UserPlaybackSettingsModel
    {
        public PlaybackState PlaybackState { get; set; }
        public bool IsPlaying { get; set; }
        public int MaxRequestsPerUser { get; set; }
    }
}