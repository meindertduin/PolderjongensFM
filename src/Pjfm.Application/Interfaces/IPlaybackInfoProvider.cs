using System;
using System.Collections.Generic;
using Pjfm.Application.Common.Dto;

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