using System;
using Pjfm.Application.Identity;
using Pjfm.Domain.Enums;

namespace Pjfm.Domain.Entities
{
    public class TopTrack
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string[] Artists { get; set; }
        public TopTrackTerm Term { get; set; }
        public int SongDurationMs { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime TimeAdded { get; set; }
    }
}