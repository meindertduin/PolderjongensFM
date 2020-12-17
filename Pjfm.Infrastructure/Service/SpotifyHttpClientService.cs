using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Spotify.Commands;
using Pjfm.Application.Spotify.Queries;
using Pjfm.Application.Test.Queries;
using Pjfm.Domain.Interfaces;
using Polly;
using Polly.Retry;
using Serilog;

namespace Pjfm.Application.Services
{
    public class SpotifyHttpClientService : ISpotifyHttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;
        private RetryPolicy _retryPolicy;
        
        private const int MaxRequestRetries = 5;

        public SpotifyHttpClientService(HttpClient httpClient, IServiceProvider serviceProvider)
        {
            _httpClient = httpClient;
            _serviceProvider = serviceProvider;
            _retryPolicy = Policy
                .Handle<InvalidOperationException>()
                .WaitAndRetry(
                    MaxRequestRetries, 
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
                    (exception, timeSpan, context) => {
                        Log.Error(exception.Message);
                    }
                );
        }

        public async Task<HttpResponseMessage> SendAuthenticatedRequest(HttpRequestMessage requestMessage, string userId)
        {
            return await _retryPolicy.Execute(async () =>
            {
                var result = await _httpClient.SendAsync(requestMessage);
                
                if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                
                    var refreshResult = await mediator.Send(new AccessTokenRefreshCommand()
                    {
                        UserId = userId,
                    });

                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.Data);
                    return await SendAuthenticatedRequest(requestMessage, userId);
                }
                
                return result;
            });
        }
    }
}