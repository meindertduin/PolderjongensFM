﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Common;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Services
{
    public class SpotifyPlayerService : ISpotifyPlayerService
    {
        private readonly ISpotifyHttpClientService _httpClientService;
        private readonly string _apibaseAddress = "https://api.spotify.com";

        public SpotifyPlayerService(ISpotifyHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }
        
        public Task<HttpResponseMessage> Play(string userId, string accessToken, string deviceId, PlayRequestDto playRequestDto = null)
        {
            var requestMessage = new HttpRequestMessage();
            
            requestMessage.Method = HttpMethod.Put;
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (string.IsNullOrEmpty(deviceId))
            {
                requestMessage.RequestUri = new Uri("https://api.spotify.com/v1/me/player/play");
            }
            else
            {
                requestMessage.RequestUri = new Uri(String.Concat("https://api.spotify.com/v1/me/player/play", $"?device_id={deviceId}"));
            }
            
            if (playRequestDto != null)
            {
                var jsonString = JsonConvert.SerializeObject(playRequestDto, new JsonSerializerSettings()
                {
                    ContractResolver = new UnderScorePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented,
                });
            
                requestMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            }
            
            return _httpClientService.SendAuthenticatedRequest(requestMessage, userId);
        }

        public Task<HttpResponseMessage> AddTrackToQueue(string userId, string accessToken, string trackId ,string deviceId = null)
        {
            var requestMessage = new HttpRequestMessage();
            
            requestMessage.Method = HttpMethod.Post;
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var requestUri = $"https://api.spotify.com/v1/me/player/queue?uri=spotify:track:{trackId}";
            
            if (string.IsNullOrEmpty(deviceId) == false)
            {
                requestUri.Concat($"&device_id={deviceId}");
            }
            
            requestMessage.RequestUri = new Uri(requestUri);

            return _httpClientService.SendAuthenticatedRequest(requestMessage, userId);
        }

        public Task<HttpResponseMessage> SkipSong(string userId, string accessToken, string deviceId = null)
        {
            var requestMessage = new HttpRequestMessage();
            
            requestMessage.Method = HttpMethod.Post;
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var requestUri = $"https://api.spotify.com/v1/me/player/next";

            if (string.IsNullOrEmpty(deviceId) == false)
            {
                requestUri.Concat($"?device_id={deviceId}");
            }

            return _httpClientService.SendAuthenticatedRequest(requestMessage, userId);
        }
    }
}