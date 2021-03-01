using System.Collections.Generic;
using Pjfm.Domain.Enums;
using Pjfm.WebClient.Services;

namespace Pjfm.Application.Common.Dto
{
    public class PlaybackSettingsDto
    {
        public bool IsPlaying { get; set; }
        public TopTrackTermFilter PlaybackTermFilter { get; set; }
        public PlaybackState PlaybackState { get; set; }
        public List<ApplicationUserDto> IncludedUsers { get; set; }
        public int MaxRequestsPerUser { get; set; }
        public int ListenersCount { get; set; }
        public BrowserQueueSettings BrowserQueueSettings { get; set; }
    }
}