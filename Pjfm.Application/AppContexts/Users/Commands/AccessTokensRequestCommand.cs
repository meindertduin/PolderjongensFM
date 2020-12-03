﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Common;
using Microsoft.Extensions.Configuration;

namespace Pjfm.Application.Spotify.Commands
{
    public class AccessTokensRequestCommand : IRequestWrapper<AccessTokensRequestResult>
    {
        public string Code { get; set; }
        public string RedirectUri { get; set; }
    }
    
    public class AccessTokenRequestCommandHandler : IHandlerWrapper<AccessTokensRequestCommand, AccessTokensRequestResult>
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _configuration;

        public AccessTokenRequestCommandHandler(IHttpClientFactory factory, IConfiguration configuration)
        {
            _factory = factory;
            _configuration = configuration;
        }
        
        public async Task<Response<AccessTokensRequestResult>> Handle(AccessTokensRequestCommand request, CancellationToken cancellationToken)
        {
            var client = _factory.CreateClient();
            var authString = Encoding.ASCII.GetBytes($"{_configuration["Spotify:ClientId"]}:{_configuration["Spotify:ClientSecret"]}");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authString));
            
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("code", request.Code), 
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", request.RedirectUri), 
            });

            try
            {
                var result = await client.PostAsync("https://accounts.spotify.com/api/token", formContent);

                var contentString = await result.Content.ReadAsStringAsync();
                
                var resultContent =
                    JsonConvert.DeserializeObject<AccessTokensRequestResult>(contentString, new JsonSerializerSettings()
                    {
                        ContractResolver = new UnderScorePropertyNamesContractResolver(),
                    });
                
                return Response.Ok("retrieving access tokens succeeded", resultContent);
            }
            catch (Exception e)
            {
                return Response.Fail<AccessTokensRequestResult>("something went wrong retrieving access tokens");
            }
        }
    }
}