using System;
using System.Collections.Generic;
using Pjfm.Application.Common.Dto;

namespace pjfm.Models
{
    public abstract class PlayerUpdateInfoModel
    {
        public TrackDto CurrentPlayingTrack { get; set; }
        public DateTime StartingTime { get; set; }
        public List<TrackDto> FillerQueuedTracks { get; set; }
        
    }
}