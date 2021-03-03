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