using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pjfm.Application.Configuration;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using pjfm.Models;

namespace Pjfm.Application.Auth.Querys
{
    public class IdentityProfileQuery : IRequestWrapper<IdentityProfileViewModel>
    {
        public ClaimsPrincipal UserClaimPrincipal { get; set; }
    }
    
    public class IdentityProfileQueryHandler : IHandlerWrapper<IdentityProfileQuery, IdentityProfileViewModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityProfileQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        public async Task<Response<IdentityProfileViewModel>> Handle(IdentityProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(request.UserClaimPrincipal);

            var claims = await _userManager.GetClaimsAsync(user);

            var isMod = claims.Any(x => x.Type == ApplicationIdentityConstants.Claims.Role
                                        && x.Value == ApplicationIdentityConstants.Roles.Mod);
            
            var isSpotifyAuthenticated = claims.Any(x => x.Type == SpotifyIdentityConstants.Claims.SpStatus
                                                         && x.Value == SpotifyIdentityConstants.Roles.Auth);
            
            if (user != null)
            {
                return Response.Ok("user successfully retrieved", new IdentityProfileViewModel()
                {
                    UserProfile = new UserProfileViewModel()
                    {
                        Id = user.Id,
                        DisplayName = user.DisplayName,
                        Email = user.Email,
                    },
                    IsMod = isMod,
                    IsSpotifyAuthenticated = isSpotifyAuthenticated,
                    EmailConfirmed = user.EmailConfirmed,
                });
            }
            return Response.Fail<IdentityProfileViewModel>("user could not be found");
        }
    }
}