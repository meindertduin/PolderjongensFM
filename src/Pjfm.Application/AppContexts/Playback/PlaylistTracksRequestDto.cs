namespace Pjfm.Application.AppContexts.Playback
{
    public class PlaylistTracksRequestDto
    {
        public string PlaylistId { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}