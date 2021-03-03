using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Common.Mediatr;
using Pjfm.Application.Common.Mediatr.Wrappers;
using Pjfm.Domain.Entities;

namespace Pjfm.Application.AppContexts.Users.Queries
{
    public class GetAllPjMembersQuery : IRequestWrapper<List<ApplicationUserDto>>
    {
        
    }
    
    public class GetALlPjMembersQueryHandler : IHandlerWrapper<GetAllPjMembersQuery, List<ApplicationUserDto>>
    {
        private readonly IAppDbContext _ctx;

        public GetALlPjMembersQueryHandler(IAppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public Task<Response<List<ApplicationUserDto>>> Handle(GetAllPjMembersQuery request, CancellationToken cancellationToken)
        {
            var result = _ctx.ApplicationUsers
                .AsNoTracking()
                .ProjectTo<ApplicationUserDto>(new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ApplicationUser, ApplicationUserDto>();
                }))
                .Where(x => x.Member)
                .ToList();

            return Task.FromResult(Response.Ok("query successful", result));
        }
    }
}