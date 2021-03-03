using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Common.Mediatr;
using Pjfm.Application.Common.Mediatr.Wrappers;
using Pjfm.Domain.Entities;

namespace Pjfm.Application.AppContexts.Users.Queries
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