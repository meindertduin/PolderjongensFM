namespace Pjfm.Application.Common.Dto
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public bool Member { get; set; }
        public bool SpotifyAuthenticated { get; set; }
    }
}