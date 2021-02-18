using System.Collections.Generic;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Entities;

namespace pjfm.Models
{
    public class DjPlaybackInfoModel : PlayerUpdateInfoModel
    {
        public List<TrackDto> SecondaryQueuedTracks { get; set; }
        public List<TrackDto> PriorityQueuedTracks { get; set; }
    }
}