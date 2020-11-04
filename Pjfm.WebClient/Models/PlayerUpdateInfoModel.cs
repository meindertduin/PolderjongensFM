using System;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Entities;

namespace pjfm.Models
{
    public class PlayerUpdateInfoModel
    {
        public TrackDto CurrentPlayingTrack { get; set; }
        public DateTime StartingTime { get; set; }
    }
}