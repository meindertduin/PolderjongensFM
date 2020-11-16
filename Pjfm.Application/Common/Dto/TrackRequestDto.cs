using Pjfm.Application.Common.Dto;

namespace pjfm.Models
{
    public class TrackRequestDto
    {
        public TrackDto Track { get; set; }
        public ApplicationUserDto User { get; set; }
    }
}