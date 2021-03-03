namespace Pjfm.Application.AppContexts.Playlists
{
    public class TopTracksRequestDto
    {
        public string Term { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}