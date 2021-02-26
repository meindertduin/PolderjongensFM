﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;
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

        public Task<HttpResponseMessage> GetUserTopTracks(string userId, string accessToken, int term)
        {
            string[] terms = {"short_term", "medium_term", "long_term" };
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"https://api.spotify.com/v1/me/top/tracks?limit=50&time_range={terms[term]}")
            };

            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId, accessToken);
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
            
            request.RequestUri = new Uri(requestUri);
            
            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId, accessToken);
        }

        public Task<HttpResponseMessage> GetTrackInfo(string userId, string accessToken, string trackId)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri($"https://api.spotify.com/v1/tracks/{trackId}");

            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId, accessToken);
        }

        public Task<HttpResponseMessage> Me(string userId, string accessToken)
        {
            var request = new HttpRequestMessage();
            
            request.Method = HttpMethod.Get;
            
            request.RequestUri = new Uri($"https://api.spotify.com/v1/me");
            
            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId, accessToken);
        }

        public Task<HttpResponseMessage> GetUserPlaylists(string userId, string accessToken, PlaylistRequestDto playlistRequest)
        {
            var request = new HttpRequestMessage();

            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"https://api.spotify.com/v1/me/playlists?limit={playlistRequest.Limit}&offset={playlistRequest.Offset}"); 
            
            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId, accessToken);
        }

        public Task<HttpResponseMessage> GetPlaylistTracks(string userId, string accessToken, PlaylistTracksRequestDto playlistTracksRequestDto)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"https://api.spotify.com/v1/playlists/{playlistTracksRequestDto.PlaylistId}/tracks" +
                                         $"?limit={playlistTracksRequestDto.Limit}" +
                                         $"&offset={playlistTracksRequestDto.Offset}");
            
            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId, accessToken);
        }

        public Task<HttpResponseMessage> GetTopTracks(string userId, string accessToken, TopTracksRequestDto topTracksRequestDto)
        {
            var request = new HttpRequestMessage();
            
            request.Method = HttpMethod.Get;
            
            request.RequestUri = new Uri($"https://api.spotify.com/v1/me/top/tracks" +
                                          $"?time_range={topTracksRequestDto.Term}" +
                                          $"&limit={topTracksRequestDto.Limit}" +
                                          $"&offset={topTracksRequestDto.Offset}");
            
            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId, accessToken);
        }

        // Overload with pagination support
        public Task<HttpResponseMessage> CustomRequest(string userId, string accessToken, Uri nextUri)
        {
            var request = new HttpRequestMessage();
            
            request.Method = HttpMethod.Get;
            request.RequestUri = nextUri;

            return _spotifyHttpClientService.SendAuthenticatedRequest(request, userId, accessToken);
        }
    }
}