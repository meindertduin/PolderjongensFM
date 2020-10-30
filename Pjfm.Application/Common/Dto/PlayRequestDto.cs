namespace Pjfm.Application.Common.Dto
{
    public class PlayRequestDto
    {
        public string ContextUri { get; set; }
        public string[] Uris { get; set; }
        public object Offset { get; set; }
        public int PositionMs { get; set; }
    }
}