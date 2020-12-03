﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Common;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Services
{
    public class SpotifyBrowserService : ISpotifyBrowserService
    {
        private readonly ISpotifyHttpClientService _spotifyHttpClientService;

        public SpotifyBrowserService(ISpotifyHttpClientService spotifyHttpClientService)
        {
            _spotifyHttpClientService = spotifyHttpClientService;
        }

        public Task<HttpResponseMessage> Search(string userId , string accessToken, SearchRequestDto searchRequestInfo)
        {
            var request = new HttpRequestMessage();
            var requestUri = $"https://api.spotify.com/v1/search?q={searchRequestInfo.Query}&type={searchRequestInfo.Type}";

            if (searchRequestInfo.Limit > 0)
            {
                requestUri.Concat($"&limit={searchRequestInfo.Limit}");
            }

            if (searchRequestInfo.Offset > 0)
            {
                requestUri.Concat($"&offset={searchRequestInfo.Offset}");
            }
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.RequestUri = new Uri(requestUri);
            
            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId);
        }

        public Task<HttpResponseMessage> GetTrackInfo(string userId, string accessToken, string trackId)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri($"https://api.spotify.com/v1/tracks/{trackId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId);
        }

        public Task<HttpResponseMessage> GetUserPlaylists(string userId, string accessToken, PlaylistRequestDto playlistRequest)
        {
            var request = new HttpRequestMessage();

            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"https://api.spotify.com/v1/me/playlists?limit={playlistRequest.Limit}&offset={playlistRequest.Offset}"); 

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId);
        }

        public Task<HttpResponseMessage> GetPlaylistTracks(string userId, string accessToken, PlaylistTracksRequestDto playlistTracksRequestDto)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"https://api.spotify.com/v1/playlists/{playlistTracksRequestDto.PlaylistId}/tracks" +
                                         $"?limit={playlistTracksRequestDto.Limit}" +
                                         $"&offset={playlistTracksRequestDto.Offset}");
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId);
        }
    }
}