using System;
using System.ComponentModel.DataAnnotations;
using Pjfm.Application.Identity;
using Pjfm.Domain.Enums;

namespace Pjfm.Domain.Entities
{
    public class TopTrack
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string SpotifyTrackId { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        public string[] Artists { get; set; }
        public TopTrackTerm Term { get; set; }
        public int SongDurationMs { get; set; }
        [MaxLength(50)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime TimeAdded { get; set; }
    }
}