using System;
using System.Collections.Generic;
using Pjfm.Application.AppContexts.Tracks;

namespace Pjfm.Api.Models
{
    public abstract class PlayerUpdateInfoModel
    {
        public TrackDto CurrentPlayingTrack { get; set; }
        public DateTime StartingTime { get; set; }
        public List<TrackDto> FillerQueuedTracks { get; set; }
        
    }
}