using System;
using System.Collections.Generic;
using Pjfm.Application.AppContexts.Playback;
using Pjfm.Application.AppContexts.Tracks;
using Pjfm.Application.AppContexts.Users;

namespace Pjfm.Application.Interfaces
{
    public interface IPlaybackInfoProvider
    {
        List<ApplicationUserDto> GetIncludedUsers();
        List<TrackDto> GetPriorityQueueTracks();
        List<TrackDto> GetSecondaryQueueTracks();
        List<TrackDto> GetFillerQueueTracks();
        Tuple<TrackDto, DateTime> GetPlayingTrackInfo();
        PlaybackSettingsDto GetPlaybackSettings();
        bool IsPlaying();
    }
}