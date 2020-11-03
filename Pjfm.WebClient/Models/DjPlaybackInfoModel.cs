using System.Collections.Generic;
using Pjfm.Domain.Entities;

namespace pjfm.Models
{
    public class DjPlaybackInfoModel : PlayerUpdateInfoModel
    {
        public List<TopTrack> PriorityQueuedTracks { get; set; }
        public List<TopTrack> FillerQueuedTracks { get; set; }
    }
}