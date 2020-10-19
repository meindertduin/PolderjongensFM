using Pjfm.Domain.Enums;

namespace Pjfm.Domain.Entities
{
    public class TopTrack
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Artists { get; set; }
        public TopTrackTerm Term { get; set; }
    }
}