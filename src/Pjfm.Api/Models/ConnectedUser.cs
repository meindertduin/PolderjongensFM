using Pjfm.Application.AppContexts.Playback;
using Pjfm.Domain.Entities;

namespace Pjfm.Api.Models
{
    public class ConnectedUser
    {
        public PlaybackDevice PlaybackDevice { get; set; }
        public ApplicationUser User { get; set; }
    }
}