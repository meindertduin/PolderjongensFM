using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Common;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Enums;

namespace Pjfm.Infrastructure.Service
{
    public class SpotifyTrackSerializer
    {
        public List<TrackDto> ConvertObject(string jsonString)
        {
            JObject objectResult = JsonConvert.DeserializeObject<dynamic>(jsonString, new JsonSerializerSettings()
            {
                ContractResolver = new UnderScorePropertyNamesContractResolver()
            });

            return MapTopTrackItems(objectResult);
        }
        
        private List<TrackDto> MapTopTrackItems(dynamic trackResult)
        {
            List<TrackDto> topTracks = new List<TrackDto>();
            
            foreach (var track in trackResult.tracks.items)
            {
                List<string> artistNames = new List<string>();

                foreach (var artist in track.artists)
                {
                    artistNames.Add((string) artist.name);
                }

                topTracks.Add(new TrackDto
                {
                    Title = track.name,
                    Artists = artistNames.ToArray(),
                    SongDurationMs = track.duration_ms,
                });
            }

            return topTracks;
        }
    }
}