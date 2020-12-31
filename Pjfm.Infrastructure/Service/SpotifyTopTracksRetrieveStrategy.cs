using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pjfm.Application.Spotify.Commands;
using Pjfm.Domain.Common;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Enums;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Infrastructure.Service
{
    public class SpotifyTopTracksRetrieveStrategy : IRetrieveStrategy
    {
        private readonly IMediator _mediator;
        private readonly ISpotifyHttpClientService _spotifyHttpClientService;
        private readonly string[] terms = {"short_term", "medium_term", "long_term" };

        public SpotifyTopTracksRetrieveStrategy(IMediator mediator, ISpotifyHttpClientService spotifyHttpClientService)
        {
            _mediator = mediator;
            _spotifyHttpClientService = spotifyHttpClientService;
        }

        public async Task<List<TopTrack>> RetrieveItems(string accessToken, int term, string userId)
        {
            var trackResult = await GetSpotifyTrackInfo(term, accessToken, userId);
                
            var topTracks = new List<TopTrack>();
            
            MapTopTrackItems(trackResult, term, userId, topTracks);

            return topTracks;
        }
        
        private async Task<dynamic> GetSpotifyTrackInfo(int term, string accessToken, string userId)
        {
            var httpRequest = new HttpRequestMessage {Method = HttpMethod.Get};
            httpRequest.RequestUri = new Uri($"https://api.spotify.com/v1/me/top/tracks?limit=50&time_range={terms[term]}");
            
            var response = await _spotifyHttpClientService.SendAuthenticatedRequest(httpRequest, userId);

            var jsonResult = await response.Content.ReadAsStringAsync();
            
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
                    SpotifyTrackId = item.id,
                    Title = item.name,
                    Artists = artistNames.ToArray(),
                    Term = (TopTrackTerm) term,
                    ApplicationUserId = userId,
                    TimeAdded = DateTime.Now,
                    SongDurationMs = item.duration_ms,
                });
            }
        }
    }
}