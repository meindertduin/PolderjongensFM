using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private readonly IServiceProvider _serviceProvider;
        private AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        private const int MaxRequestRetries = 5;

        public SpotifyHttpClientService(HttpClient httpClient, IServiceProvider serviceProvider)
        {
            _httpClient = httpClient;
            _serviceProvider = serviceProvider;
            
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(1, async (result, retryCount, context) =>
                {
                    if (context.ContainsKey("refresh_token"))
                    {
                        using var scope = serviceProvider.CreateScope();
                        var mediator = scope.ServiceProvider.GetService<IMediator>();

                        var refreshResponse = await mediator.Send(new AccessTokenRefreshCommand()
                        {
                            UserId = context["user_id"] as string,
                        });

                        if (refreshResponse.Error == false)
                        {
                            context["access_token"] = refreshResponse.Data;
                        }
                    }
                });
        }

        public async Task<HttpResponseMessage> SendAuthenticatedRequest(HttpRequestMessage requestMessage, string userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(userId);

            return await _retryPolicy.ExecuteAsync(async context =>
            {
                var message = await requestMessage.CloneAsync();
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context["access_token"] as string);
                
                return await _httpClient.SendAsync(message);
            }, new Dictionary<string, object>()
            {
                {"access_token", user.SpotifyAccessToken},
                {"refresh_token", user.SpotifyRefreshToken},
                { "user_id", user.Id },
            });
        }
    }
}