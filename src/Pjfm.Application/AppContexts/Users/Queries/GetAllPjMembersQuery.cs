using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.MediatR.Users.Queries
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