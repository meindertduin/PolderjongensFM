using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pjfm.Application.AppContexts.Spotify;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Mappings;
using Pjfm.Application.MediatR;
using Pjfm.Application.Services;
using Pjfm.Domain.Common;
using Pjfm.WebClient.Services;
using Pjfm.WebClient.Services.FillerQueueState;

namespace Pjfm.Api.Services.SpotifyPlayback.FillerQueueState
{
    public class GenreBrowsingState : FillerQueueStateBase, IFillerQueueState
    {
        private readonly PlaybackQueue _playbackQueue;
        private readonly ISpotifyBrowserService _spotifyBrowserService;

        public GenreBrowsingState(PlaybackQueue playbackQueue, ISpotifyBrowserService spotifyBrowserService)
        {
            _playbackQueue = playbackQueue;
            _spotifyBrowserService = spotifyBrowserService;
        }
        public async Task<Response<List<TrackDto>>> RetrieveFillerTracks(int amount)
        {
            var settings = _playbackQueue.PlaybackQueueSettings;
            var recommendedSettings = new RecommendationsSettings()
            {
                Limit = amount,
                SeedGenres = "black_metal",
                SeedArtists = "0sfWl1dWLgEtMy9oFnNoDA",
                SeedTracks = "3Op2bVsGwXrHxWs7XhR5bX",
                MaxPopularity = 50,
                MinInstrumentalness = 0.95m,
            };
            var response = await _spotifyBrowserService.GetRecommendations(recommendedSettings);

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                JObject objectResult = JsonConvert.DeserializeObject<dynamic>(jsonData, new JsonSerializerSettings()
                {
                    ContractResolver = new UnderScorePropertyNamesContractResolver(),
                });

                var mapper = new TrackDtoMapper();
                var tracks = mapper.MapObjects(objectResult);

                return Response.Ok("retrieving tracks was succesfull", tracks);
            }
            
            return Response.Fail<List<TrackDto>>("failed to retrieve tracks");
        }
    }
}