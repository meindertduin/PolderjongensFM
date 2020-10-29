using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Common;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Spotify.Commands
{
    public class AccessTokenRefreshCommand : IRequestWrapper<string>
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string UserId { get; set; }
    }
    
    public class AccessTokenRefreshCommandHandler : IHandlerWrapper<AccessTokenRefreshCommand, string>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAppDbContext _appDbContext;

        public AccessTokenRefreshCommandHandler(IHttpClientFactory clientFactory, IAppDbContext appDbContext)
        {
            _clientFactory = clientFactory;
            _appDbContext = appDbContext;
        }
        
        public async Task<Response<string>> Handle(AccessTokenRefreshCommand request, CancellationToken cancellationToken)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var authString = Encoding.ASCII.GetBytes($"{request.ClientId}:{request.ClientSecret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authString));

                var user = _appDbContext.ApplicationUsers.FirstOrDefault(u => u.Id == request.UserId);
                
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
                        return Response.Ok<string>("Accesstoken successfully retireved", resultContent.AccessToken);
                    }
                }
                
                return Response.Fail<string>("Accesstoken could not be retrieved");
            }
            
        }
    }
}