using System.Linq;
using AutoMapper;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Entities;

namespace Pjfm.Application.Mappings
{
    public class TopTrackMappingProfile : Profile
    {
        public TopTrackMappingProfile()
        {
            CreateMap<TopTrack, TrackDto>()
                .ForMember(dest => dest.User, opts => opts.MapFrom(src => src.ApplicationUser));
        }
    }
}