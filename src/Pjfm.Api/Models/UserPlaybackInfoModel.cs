using System.Collections.Generic;
using Pjfm.Application.AppContexts.Tracks;

namespace Pjfm.Api.Models
{
    public class UserPlaybackInfoModel : PlayerUpdateInfoModel
    {
        public List<TrackDto> SecondaryQueuedTracks { get; set; }
        public List<TrackDto> PriorityQueuedTracks { get; set; }
        
    }
}