﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pjfm.Application.Configuration;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Common;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Spotify.Commands
{
    public class AccessTokenRefreshCommand : IRequestWrapper<string>
    {
        public string UserId { get; set; }
    }
    
    public class AccessTokenRefreshCommandHandler : IHandlerWrapper<AccessTokenRefreshCommand, string>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccessTokenRefreshCommandHandler(IHttpClientFactory clientFactory, IAppDbContext appDbContext, 
            IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _clientFactory = clientFactory;
            _appDbContext = appDbContext;
            _configuration = configuration;
            _userManager = userManager;
        }
        
        public async Task<Response<string>> Handle(AccessTokenRefreshCommand request, CancellationToken cancellationToken)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var authString = Encoding.ASCII.GetBytes($"{_configuration["Spotify:ClientId"]}:{_configuration["Spotify:ClientSecret"]}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authString));

                var user = _appDbContext.ApplicationUsers.FirstOrDefault(u => u.Id == request.UserId);

                if (user != null)
                {
                    FormUrlEncodedContent formContent = new FormUrlEncodedContent(new []
                    {
                        new KeyValuePair<string, string>("grant_type", "refresh_token"),
                        new KeyValuePair<string, string>("refresh_token", user.SpotifyRefreshToken), 
                    });

                    var result = await client.PostAsync("https://accounts.spotify.com/api/token", formContent);

                    if (result.IsSuccessStatusCode)
                    {
                        var resultString = await result.Content.ReadAsStringAsync();
                
                        var resultContent =
                            JsonConvert.DeserializeObject<RefreshAccessTokenRequestResult>(resultString, new JsonSerializerSettings()
                            {
                                ContractResolver = new UnderScorePropertyNamesContractResolver(),
                            });

                        if (resultContent != null)
                        {
                            user.SpotifyAccessToken = resultContent.AccessToken;
                            await _appDbContext.SaveChangesAsync(cancellationToken);
                            return Response.Ok("Accesstoken successfully retireved", resultContent.AccessToken);
                        }
                    }

                    // refresh token is invalid due to being withdrawn
                    if (result.StatusCode == HttpStatusCode.BadRequest)
                    {
                        user.SpotifyAuthenticated = false;
                        await _userManager.RemoveClaimAsync(user, new Claim(SpotifyIdentityConstants.Claims.SpStatus,
                            SpotifyIdentityConstants.Roles.Auth));
                        
                        var userTopTracks = _appDbContext.TopTracks.Where(track => track.ApplicationUserId == user.Id);
                        _appDbContext.TopTracks.RemoveRange(userTopTracks);
                        await _appDbContext.SaveChangesAsync(cancellationToken);
                    }
                }

                return Response.Fail<string>("Accesstoken could not be refreshed");
            }
            
        }
    }
}