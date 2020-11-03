using System;
using Pjfm.Domain.Entities;

namespace pjfm.Models
{
    public class PlayerUpdateInfoModel
    {
        public TopTrack CurrentPlayingTrack { get; set; }
        public DateTime StartingTime { get; set; }
    }
}