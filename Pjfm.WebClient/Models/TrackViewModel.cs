using System;
using Pjfm.Application.Identity;
using Pjfm.Domain.Enums;

namespace pjfm.Models
{
    public class TrackViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string[] Artists { get; set; }
        public int SongDurationMs { get; set; }
        public UserViewModel ApplicationUser { get; set; }
    }
}