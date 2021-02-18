namespace Pjfm.Application.Common.Dto
{
    public class TopTracksRequestDto
    {
        public string Term { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}