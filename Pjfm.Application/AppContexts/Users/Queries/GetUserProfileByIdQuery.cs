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