using Pjfm.Application.AppContexts.Users;

namespace Pjfm.Application.AppContexts.Tracks
{
    public class TrackRequestDto
    {
        public TrackDto Track { get; set; }
        public ApplicationUserDto User { get; set; }
    }
}