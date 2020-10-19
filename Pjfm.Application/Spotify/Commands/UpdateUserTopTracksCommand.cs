using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pjfm.Application.Common;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Common;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Enums;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Spotify.Commands
{
    public class UpdateUserTopTracksCommand : IRequestWrapper<string>
    {
        public string AccessToken { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class UpdateUserTopTracksCommandHandler : IHandlerWrapper<UpdateUserTopTracksCommand, string>
    {
        private readonly IAppDbContext _ctx;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string[] terms = new[] {"short_term", "medium_term", "long_term" };
        
        public UpdateUserTopTracksCommandHandler(IAppDbContext ctx, IHttpClientFactory httpClientFactory)
        {
            _ctx = ctx;
            _httpClientFactory = httpClientFactory;
        }
        
        public async Task<Response<string>> Handle(UpdateUserTopTracksCommand request, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory
                .CreateClient(ApplicationConstants.HttpClientNames.SpotifyApiClient);
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",request.AccessToken);

            var topTracks = await SerializeTopTracks(request, client);
            
            await _ctx.TopTracks.AddRangeAsync(topTracks, CancellationToken.None);
            await _ctx.SaveChangesAsync(CancellationToken.None);
            
            
            return Response.Ok("succeeded", "toptracks have been saved to the database");
        }

        private async Task<List<TopTrack>> SerializeTopTracks(UpdateUserTopTracksCommand request, HttpClient client)
        {
            List<TopTrack> topTracks = new List<TopTrack>();

            for (int i = 0; i < terms.Length; i++)
            {
                var trackResult = await GetSpotifyTrackInfo(client, i);

                if (trackResult != null)
                {
                    MapTopTrackItems(request, trackResult, topTracks, i);
                }
            }
            
            return topTracks;
        }
        
        private async Task<dynamic> GetSpotifyTrackInfo(HttpClient client, int i)
        {
            var jsonResult = await client.GetAsync($"v1/me/top/tracks?limit=50&time_range={terms[i]}").Result.Content
                .ReadAsStringAsync();

            var objectResult = JsonConvert.DeserializeObject<dynamic>(jsonResult, new JsonSerializerSettings()
            {
                ContractResolver = new UnderScorePropertyNamesContractResolver()
            });
            return objectResult;
        }

        private void MapTopTrackItems(UpdateUserTopTracksCommand request, dynamic trackResult, List<TopTrack> topTracks, int i)
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
                    Term = (TopTrackTerm) i,
                    ApplicationUserId = request.User.Id,
                });
            }
        }
    }
}