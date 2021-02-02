using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;

namespace pjfm.Models
{
    public class ConnectedUser
    {
        public PlaybackDevice PlaybackDevice { get; set; }
        public ApplicationUser User { get; set; }
    }
}