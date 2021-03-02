using System;
using System.Collections.Generic;
using System.Linq;
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
            var recommendedSettings = GetRecommendationsSettings(amount);
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

        private RecommendationsSettings GetRecommendationsSettings(int amount)
        {
            var settings = _playbackQueue.GetBrowserQueueSettings();
            
            var tempoValues = settings.GetTempoValues();
            var instrumentalnessValues = settings.GetInstrumentalnessValues();
            var energyValues = settings.GetEnergyValues();
            var danceAbilityValues = settings.GetDanceAbilityValues();
            var popularityValues = settings.GetPopularityValues();

            return new RecommendationsSettings()
            {
                Limit = amount,
                SeedGenres = String.Join(",", settings.Genres.Distinct()),
                SeedArtists = String.Join(",", settings.SeedArtists.Distinct()),
                MinTempo = tempoValues.Min,
                MaxTempo = tempoValues.Max,
                TargetTempo = tempoValues.Target,
                MinInstrumentalness = instrumentalnessValues.Min,
                MaxInstrumentalness = instrumentalnessValues.Max,
                TargetInstrumentalness = instrumentalnessValues.Target,
                MinEnergy = energyValues.Min,
                MaxEnergy = energyValues.Max,
                TargetEnergy = energyValues.Target,
                MinDanceability = danceAbilityValues.Min,
                MaxDanceability = danceAbilityValues.Max,
                TargetDanceability = danceAbilityValues.Target,
                MinPopularity = popularityValues.Min,
                MaxPopularity = popularityValues.Max,
                TargetPopularity = popularityValues.Target,
            };
        }
    }
}