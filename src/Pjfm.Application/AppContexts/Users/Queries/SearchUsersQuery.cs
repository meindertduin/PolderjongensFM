using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.MediatR.Users.Queries
{
    public class SearchUsersQuery : IRequestWrapper<List<ApplicationUserDto>>
    {
        public string QueryString { get; set; }
    }
    
    public class SearchUsersQueryHandler : IHandlerWrapper<SearchUsersQuery, List<ApplicationUserDto>>
    {
        private readonly IAppDbContext _ctx;

        public SearchUsersQueryHandler(IAppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public Task<Response<List<ApplicationUserDto>>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            var users = _ctx.ApplicationUsers
                .AsNoTracking()
                .Where(x => x.UserName.Contains(request.QueryString) || x.DisplayName.Contains(request.QueryString))
                .ProjectTo<ApplicationUserDto>(new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ApplicationUser, ApplicationUserDto>();
                }))
                .ToList();

            return Task.FromResult(Response.Ok("Query was successful", users));
        }
    }
}