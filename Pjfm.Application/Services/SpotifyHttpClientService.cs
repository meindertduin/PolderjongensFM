using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MediatR;
using Pjfm.Application.Spotify.Commands;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Services
{
    public class SpotifyHttpClientService : ISpotifyHttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IMediator _mediator;

        public SpotifyHttpClientService(HttpClient httpClient, IMediator mediator)
        {
            _httpClient = httpClient;
            _mediator = mediator;
        }

        public async Task<HttpResponseMessage> SendAuthenticatedRequest(HttpRequestMessage requestMessage, string userId)
        {
            var result = await _httpClient.SendAsync(requestMessage);
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshResult = await _mediator.Send(new AccessTokenRefreshCommand()
                {
                    UserId = userId,
                });

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.Data);
                return await SendAuthenticatedRequest(requestMessage, userId);
            }
            return result;
        }
    }
}