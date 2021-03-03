using System.Collections.Generic;
using Pjfm.Application.AppContexts.Spotify;
using Pjfm.Application.AppContexts.Users;
using Pjfm.Domain.Enums;

namespace Pjfm.Application.AppContexts.Playback
{
    public class PlaybackSettingsDto
    {
        public bool IsPlaying { get; set; }
        public TopTrackTermFilter PlaybackTermFilter { get; set; }
        public PlaybackState PlaybackState { get; set; }
        public List<ApplicationUserDto> IncludedUsers { get; set; }
        public int MaxRequestsPerUser { get; set; }
        public int ListenersCount { get; set; }
        public FillerQueueType FillerQueueState { get; set; }
        public BrowserQueueSettings BrowserQueueSettings { get; set; }
    }
}