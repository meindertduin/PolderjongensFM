using Pjfm.Domain.Enums;

namespace Pjfm.Application.Common.Dto
{
    public class TrackDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string[] Artists { get; set; }
        public string MainArtistId { get; set; }
        public TopTrackTerm Term { get; set; }
        public TrackType TrackType { get; set; }
        public int SongDurationMs { get; set; }
        public ApplicationUserDto User { get; set; }
        public string Message { get; set; }
    }
}