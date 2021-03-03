namespace Pjfm.Application.AppContexts.Playback
{
    public class SearchRequestDto
    {
        public string Query { get; set; }
        public string Type { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}