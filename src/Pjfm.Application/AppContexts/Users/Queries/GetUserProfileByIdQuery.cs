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
    public class GetUserProfileByIdQuery : IRequestWrapper<ApplicationUserDto>
    {
        public string Id { get; set; }
    }
    
    public class GetUSerProfileByIdQueryHandler : IHandlerWrapper<GetUserProfileByIdQuery, ApplicationUserDto>
    {
        private readonly IAppDbContext _ctx;

        public GetUSerProfileByIdQueryHandler(IAppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public Task<Response<ApplicationUserDto>> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var applicationUserProfile = _ctx.ApplicationUsers
                .Where(user => user.Id == request.Id)
                .ProjectTo<ApplicationUserDto>(new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ApplicationUser, ApplicationUserDto>();
                }))
                .FirstOrDefault();

            return Task.FromResult(Response.Ok("query was successfull", applicationUserProfile));
        }
    }
}