using Pjfm.Domain.Enums;

namespace Pjfm.Application.Common.Dto
{
    public class TrackDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string[] Artists { get; set; }
        public TopTrackTerm Term { get; set; }
        public TrackType TrackType { get; set; }
        public int SongDurationMs { get; set; }
        public string UserDisplayName { get; set; }
        public string ApplicationUserId { get; set; }
    }
}