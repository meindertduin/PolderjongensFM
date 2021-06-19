using System.Security.Claims;
using Pjfm.Application.Common.Classes;

namespace Pjfm.Application.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static PjfmPrincipal GetPjfmPrincipal(this ClaimsPrincipal claimsPrincipal)
        {
            return new PjfmPrincipal(claimsPrincipal);
        }
    }
}