using AutoMapper;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Entities;

namespace Pjfm.Application.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TopTrack, TrackDto>();
        }
    }
}