using System.Collections.Generic;
using Pjfm.Application.Common.Dto;

namespace Pjfm.WebClient.Services.FillerQueueState
{
    public class FillerQueueStateBase
    {
        internal readonly List<TrackDto> RecentlyPlayed = new List<TrackDto>();
    }
}