using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Pjfm.Application.AppContexts.Spotify;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Interfaces;
using Pjfm.Domain.ValueObjects;

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

            return _spotifyHttpClientService.SendAccessTokenRequest(request, userId, accessToken);
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
            
            return _spotifyHttpClientService.SendAccessTokenRequest(request, userId, accessToken);
        }

        public Task<HttpResponseMessage> GetTrackInfo(string userId, string accessToken, string trackId)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri($"https://api.spotify.com/v1/tracks/{trackId}");

            return _spotifyHttpClientService.SendAccessTokenRequest(request, userId, accessToken);
        }

        public Task<HttpResponseMessage> Me(string userId, string accessToken)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get, RequestUri = new Uri($"https://api.spotify.com/v1/me")
            };

            return _spotifyHttpClientService.SendAccessTokenRequest(request, userId, accessToken);
        }

        public Task<HttpResponseMessage> GetUserPlaylists(string userId, string accessToken, PlaylistRequestDto playlistRequest)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri =
                    new Uri(
                        $"https://api.spotify.com/v1/me/playlists?limit={playlistRequest.Limit}&offset={playlistRequest.Offset}")
            };

            return _spotifyHttpClientService.SendAccessTokenRequest(request, userId, accessToken);
        }

        public Task<HttpResponseMessage> GetPlaylistTracks(string userId, string accessToken, PlaylistTracksRequestDto playlistTracksRequestDto)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"https://api.spotify.com/v1/playlists/{playlistTracksRequestDto.PlaylistId}/tracks" +
                                         $"?limit={playlistTracksRequestDto.Limit}" +
                                         $"&offset={playlistTracksRequestDto.Offset}");
            
            return _spotifyHttpClientService.SendAccessTokenRequest(request, userId, accessToken);
        }

        public Task<HttpResponseMessage> GetTopTracks(string userId, string accessToken, TopTracksRequestDto topTracksRequestDto)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.spotify.com/v1/me/top/tracks" +
                                     $"?time_range={topTracksRequestDto.Term}" +
                                     $"&limit={topTracksRequestDto.Limit}" +
                                     $"&offset={topTracksRequestDto.Offset}")
            };

            return _spotifyHttpClientService.SendAccessTokenRequest(request, userId, accessToken);
        }

        // Overload with pagination support
        public Task<HttpResponseMessage> CustomRequest(string userId, string accessToken, Uri nextUri)
        {
            var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = nextUri};


            return _spotifyHttpClientService.SendAccessTokenRequest(request, userId, accessToken);
        }

        public Task<HttpResponseMessage> GetRecommendations(RecommendationsSettings settings)
        {
            var request = new HttpRequestMessage() {Method = HttpMethod.Get};
            var settingsProperties = typeof(RecommendationsSettings).GetProperties();

            var uriString = new StringBuilder("https://api.spotify.com/v1/recommendations?market=NL");

            foreach (var property in settingsProperties)
            {
                if (property.GetValue(settings) != null)
                {
                    var name = property.Name.PascalToSnakeCase();
                    string value;
                    value = property.PropertyType == typeof(decimal?)
                        ? ((decimal) property.GetValue(settings)).ToString(CultureInfo.CreateSpecificCulture("en-us")) 
                        : property.GetValue(settings)?.ToString();
                    
                    uriString.Append($"&{name}={value}");
                }
            }

            request.RequestUri = new Uri(uriString.ToString());
            
            return _spotifyHttpClientService.SendClientCredentialsRequest(request);
        }

        public Task<HttpResponseMessage> ServerGetMultipleTracks(string[] trackIds)
        {
            var request = new HttpRequestMessage() {Method = HttpMethod.Get};

            var requestUri = new StringBuilder("https://api.spotify.com/v1/tracks?ids=");
            requestUri.Append(String.Join(",", trackIds));

            request.RequestUri = new Uri(requestUri.ToString());
            return _spotifyHttpClientService.SendClientCredentialsRequest(request);
        }

        public Task<HttpResponseMessage> GetSpotifyGenres()
        {
            var request = new HttpRequestMessage() {Method = HttpMethod.Get, 
                RequestUri = new Uri("https://api.spotify.com/v1/recommendations/available-genre-seeds")};

            return _spotifyHttpClientService.SendClientCredentialsRequest(request);
        }
    }
}