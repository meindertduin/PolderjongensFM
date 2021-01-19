using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Identity;
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
        private AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        
        public SpotifyHttpClientService(HttpClient httpClient, IServiceProvider serviceProvider)
        {
            _httpClient = httpClient;
            
            // creates a retry policy that will handle refreshing of access-token if expired
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(1, async (result, retryCount, context) =>
                {
                    if (context.ContainsKey("refresh_token"))
                    {
                        using var scope = serviceProvider.CreateScope();
                        var mediator = scope.ServiceProvider.GetService<IMediator>();

                        var oldMessage = (HttpRequestMessage) context["request_message"];
                        var newMessage = await oldMessage.CloneAsync();

                        var refreshResponse = await mediator.Send(new AccessTokenRefreshCommand()
                        {
                            UserId = context["user_id"] as string,
                        });

                        if (refreshResponse.Error == false)
                        {
                            newMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResponse.Data);
                            context["request_message"] = newMessage;
                        }
                    }
                });
        }

        public async Task<HttpResponseMessage> SendAuthenticatedRequest(HttpRequestMessage requestMessage, string userId, string accessToken)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            // send the requestMessage through the retry policy that will handle access-token refresh if expired
            return await _retryPolicy.ExecuteAsync(async context => 
                await _httpClient.SendAsync(context["request_message"] as HttpRequestMessage), 
                new Dictionary<string, object>()
            {
                { "user_id", userId },
                { "request_message", requestMessage }
            });
        }
    }
}