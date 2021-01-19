namespace Pjfm.Application.AppContexts.Tracks
{
    public class RandomTrackQueryEntity
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Artists { get; set; }
        public int Term { get; set; }
        public int SongDurationMs { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public bool Member { get; set; }
    }
}