using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pjfm.Application.Common;
using Pjfm.Domain.Common;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Enums;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Infrastructure.Service
{
    public class SpotifyTopTracksClient : ISpotifyTopTracksClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string[] terms = {"short_term", "medium_term", "long_term" };

        public SpotifyTopTracksClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<TopTrack>> GetTopTracks(string accessToken, int term, string userId)
        {
            using var client = _httpClientFactory.CreateClient(ApplicationConstants.HttpClientNames.SpotifyApiClient);
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                
            var trackResult = await GetSpotifyTrackInfo(client, term);
                
            var topTracks = new List<TopTrack>();
            
            MapTopTrackItems(trackResult, term, userId, topTracks);

            return topTracks;
        }
        
        private async Task<dynamic> GetSpotifyTrackInfo(HttpClient client, int i)
        {
            var jsonResult = await client.GetAsync($"v1/me/top/tracks?limit=50&time_range={terms[i]}").Result.Content
                .ReadAsStringAsync();

            JObject objectResult = JsonConvert.DeserializeObject<dynamic>(jsonResult, new JsonSerializerSettings()
            {
                ContractResolver = new UnderScorePropertyNamesContractResolver()
            });

            return objectResult;
        }
        
        private void MapTopTrackItems(dynamic trackResult, int term, string userId, List<TopTrack> topTracks)
        {
            foreach (var item in trackResult.items)
            {
                List<string> artistNames = new List<string>();

                foreach (var artist in item.artists)
                {
                    artistNames.Add((string) artist.name);
                }

                topTracks.Add(new TopTrack
                {
                    Id = item.id,
                    Title = item.name,
                    Artists = artistNames.ToArray(),
                    Term = (TopTrackTerm) term,
                    ApplicationUserId = userId,
                    TimeAdded = DateTime.Now,
                });
            }
        }
    }
}