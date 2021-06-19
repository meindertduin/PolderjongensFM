using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Pjfm.Bff
{
    public partial class Startup
    {
        private void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "cookies";
                    options.DefaultChallengeScheme = "oidc";
                    options.DefaultSignOutScheme = "oidc";
                })
                .AddCookie("cookies", options =>
                {
                    options.Cookie.Name = "bff";
                    options.Cookie.SameSite = SameSiteMode.Strict;
                }).AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://localhost:5001";
                    
                    // confidential client using code flow + PKCE + query response mode
                    options.ClientId = "pjfm_web_client";
                    options.ClientSecret = "test_secret";
                    options.ResponseType = "code id_token";
                    options.ResponseMode = "query";

                    options.SignedOutRedirectUri = "https://localhost:5005/signout-callback-oidc";

                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.UsePkce = true;

                    // save access and refresh token to enable automatic lifetime management
                    options.SaveTokens = true;
                    
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("IdentityServerApi");
                    options.Scope.Add("Role");
                    
                    // request refresh token
                    options.Scope.Add("offline_access");
                });
        }
    }
}