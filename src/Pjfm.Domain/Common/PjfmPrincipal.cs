using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Pjfm.Domain.Enums;

namespace Pjfm.Domain.Common
{
    public class PjfmPrincipal
    {
        public ClaimsPrincipal Principal { get; }
        public IIdentity Identity => Principal.Identity;
        public IEnumerable<GebrukerRol> Roles { get; }

        public PjfmPrincipal(ClaimsPrincipal principal)
        {
            Roles = GetRolesClaimValues(principal);
            
            Principal = principal;
        }

        private static IEnumerable<GebrukerRol> GetRolesClaimValues(ClaimsPrincipal principal)
        {
            var roleClaims = principal.FindAll(ClaimTypes.Role);

            if (roleClaims == null || !roleClaims.Any())
            {
                throw new InvalidOperationException($"Requested claim type {ClaimTypes.Role} could not be found");
            }

            return roleClaims.Select(x => Enum.Parse<GebrukerRol>(x.Value)).ToList();
        }
    }
}