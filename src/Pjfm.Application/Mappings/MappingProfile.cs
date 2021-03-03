using AutoMapper;
using Pjfm.Application.AppContexts.Tracks;
using Pjfm.Domain.Entities;

namespace Pjfm.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TopTrack, TrackDto>();
        }
    }
}