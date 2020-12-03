using System.Collections.Generic;
using Pjfm.Application.Common.Dto;

namespace pjfm.Models
{
    public class UserPlaybackInfoModel : PlayerUpdateInfoModel
    {
        public List<TrackDto> SecondaryQueuedTracks { get; set; }
        public List<TrackDto> PriorityQueuedTracks { get; set; }
    }
}