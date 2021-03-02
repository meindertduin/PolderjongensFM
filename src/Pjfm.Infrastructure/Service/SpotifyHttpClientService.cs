using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pjfm.Application.Spotify.Commands;
using Pjfm.Domain.Interfaces;
using Pjfm.Domain.ValueObjects;
using Polly;
using Polly.Retry;

namespace Pjfm.Application.Services
{
    public class SpotifyHttpClientService : ISpotifyHttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private AsyncRetryPolicy<HttpResponseMessage> _tokenRetryPolicy;
        private AsyncRetryPolicy _clientCredentialsRetryPolicy;

        public SpotifyHttpClientService(HttpClient httpClient, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            // creates a retry policy that will handle refreshing of access-token if expired
            _tokenRetryPolicy = Policy
                .HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(1, async (result, retryCount, context) =>
                {
                    using var scope = serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetService<IMediator>();

                    var oldMessage = context["request_message"] as HttpRequestMessage;
                    var newMessage = await oldMessage.CloneAsync();

                    var refreshResponse = await mediator.Send(new AccessTokenRefreshCommand()
                    {
                        UserId = context["user_id"] as string,
                    });

                    if (refreshResponse.Error == false)
                    {
                        newMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResponse.Data);
                    }
                        
                    context["request_message"] = newMessage;
                });

            _clientCredentialsRetryPolicy = Policy
                .Handle<HttpRequestException>()
                .RetryAsync(1);
        }

        public async Task<HttpResponseMessage> SendAccessTokenRequest(HttpRequestMessage requestMessage, string userId, string accessToken)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            // send the requestMessage through the retry policy that will handle access-token refresh if expired
            return await _tokenRetryPolicy.ExecuteAsync(async context =>
                    await _httpClient.SendAsync(context["request_message"] as HttpRequestMessage), 
                new Dictionary<string, object>()
            {
                { "user_id", userId },
                { "request_message", requestMessage }
            });
        }

        public async Task<HttpResponseMessage> SendClientCredentialsRequest(HttpRequestMessage requestMessage)
        {
            var token = await GetServerAccessToken();

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            return await _clientCredentialsRetryPolicy.ExecuteAsync(async () =>
                await _httpClient.SendAsync(requestMessage));
        }

        private async Task<string> GetServerAccessToken()
        {
            var tokenRequest = new HttpRequestMessage() {Method = HttpMethod.Post, RequestUri = new Uri("https://accounts.spotify.com/api/token")};
            
            var authString = Encoding.ASCII.GetBytes($"{_configuration["Spotify:ClientId"]}:{_configuration["Spotify:ClientSecret"]}");
            tokenRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authString));

            FormUrlEncodedContent formContent = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
            });

            tokenRequest.Content = formContent;

            var tokenResponse = await _httpClient.SendAsync(tokenRequest);
            if (tokenResponse.IsSuccessStatusCode)
            {
                var jsonData = await tokenResponse.Content.ReadAsStringAsync();
                var jsonContent = JsonConvert.DeserializeObject<dynamic>(jsonData);
                return jsonContent.access_token;
            }
            
            return String.Empty;
        }
    }
}