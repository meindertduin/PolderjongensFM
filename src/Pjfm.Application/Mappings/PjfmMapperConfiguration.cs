using System;
using AutoMapper;
using AutoMapper.Configuration;

namespace Pjfm.Application.Mappings
{
    public class PjfmMapperConfiguration : MapperConfiguration
    {
        public PjfmMapperConfiguration(MapperConfigurationExpression configurationExpression) : base(configurationExpression)
        {
            
        }

        public PjfmMapperConfiguration(Action<IMapperConfigurationExpression> configure) : base(configure)
        {
        }

        private static void Configure(IMapperConfigurationExpression config)
        {
            config.AddProfile<TopTrackMappingProfile>();
        }
    }
}