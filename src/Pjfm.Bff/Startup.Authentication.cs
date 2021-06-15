﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
                })
                .AddCookie("cookies", options =>
                {
                    options.Cookie.Name = "bff";
                    options.Cookie.SameSite = SameSiteMode.Strict;
                }).AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.ClientId = "pjfm_web_client";
                    options.ClientSecret = "test_secret";

                    options.SignedOutRedirectUri = "https://localhost:5005";
                    options.ResponseType = "code";
                    options.GetClaimsFromUserInfoEndpoint = true;
                    
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("IdentityServerApi");
                    options.Scope.Add("Role");

                    options.UsePkce = true;
                    options.SaveTokens = true;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role",
                    };
                });
        }
    }
}