using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;

namespace Pjfm.Application.Auth.Querys
{
    public class UserProfileQuery : IRequestWrapper<UserProfileViewModel>
    {
        public ClaimsPrincipal UserClaimPrincipal { get; set; }
    }
    
    public class UserProfileQueryHandler : IHandlerWrapper<UserProfileQuery, UserProfileViewModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        public async Task<Response<UserProfileViewModel>> Handle(UserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(request.UserClaimPrincipal);
            if (user != null)
            {
                return Response.Ok("user successfully retrieved", new UserProfileViewModel()
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                });
            }
            return Response.Fail<UserProfileViewModel>("user could not be found");
        }
    }
}