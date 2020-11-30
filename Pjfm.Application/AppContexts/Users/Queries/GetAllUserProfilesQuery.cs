using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.MediatR.Users.Queries
{
    public class GetAllUserProfileQuery : IRequestWrapper<List<ApplicationUserDto>>
    {
    }
    
    public class GetAllUserProfileQueryHandler : IHandlerWrapper<GetAllUserProfileQuery, List<ApplicationUserDto>>
    {
        private readonly IAppDbContext _ctx;

        public GetAllUserProfileQueryHandler(IAppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public Task<Response<List<ApplicationUserDto>>> Handle(GetAllUserProfileQuery request, CancellationToken cancellationToken)
        {
            var userProfiles = _ctx.ApplicationUsers
                .ProjectTo<ApplicationUserDto>(new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ApplicationUser, ApplicationUserDto>();
                }))
                .ToList();

            return Task.FromResult(Response.Ok("Query was successful", userProfiles));
        }
    }
}