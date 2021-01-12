using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Pjfm.Application.Identity
{
    public static class ApplicationIdentityConfiguration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(), 
                new IdentityResources.Profile(), 
                new IdentityResource(ApplicationIdentityConstants.Claims.Role, 
                    userClaims: new []{ ApplicationIdentityConstants.Claims.Role }, 
                    displayName: "role"),
            };
        }
        
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            
            return new List<ApiScope>
            {
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName, new[]
                {
                    JwtClaimTypes.PreferredUserName,
                    ApplicationIdentityConstants.Claims.Role,
                })
            };
        }
        
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "pjfm_web_client",
                    AllowedGrantTypes = GrantTypes.Code,
                    
                    RedirectUris = new[]
                    {
                        "https://localhost:8085/oidc-callback",
                        "https://localhost:8085/oidc-client-silent-renew.html",
                    },
                    PostLogoutRedirectUris = new[]
                    {
                        "https://localhost:8085",
                    },
                    
                    AllowedScopes = new[]
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.LocalApi.ScopeName,
                        ApplicationIdentityConstants.Claims.Role,
                    },
                    AllowedCorsOrigins = new List<string>()
                    {
                        "https://localhost:8085",
                    },

                    AlwaysIncludeUserClaimsInIdToken = true,
                    
                    RequirePkce = true,
                    
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false,
                }
            };
        }
    }
}