using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Pjfm.Application.Identity;
using Pjfm.Domain.Enums;

namespace Pjfm.Application.Common.Classes
{
    public class PjfmPrincipal
    {
        public ClaimsPrincipal Principal { get; }
        public IIdentity Identity => Principal.Identity;
        public IEnumerable<UserRole> Roles { get; }
        public bool SpotifyAuthenticated { get; }

        public PjfmPrincipal(ClaimsPrincipal principal)
        {
            Roles = GetRolesClaimValues(principal);
            SpotifyAuthenticated = principal.HasClaim(ApplicationIdentityConstants.Claims.SpAuth, ApplicationIdentityConstants.Roles.Auth);
            Principal = principal;
        }

        private static IEnumerable<UserRole> GetRolesClaimValues(ClaimsPrincipal principal)
        {
            var roleClaims = principal.FindAll("Role");

            return roleClaims.Select(x => Enum.Parse<UserRole>(x.Value)).ToList();
        }
    }
}