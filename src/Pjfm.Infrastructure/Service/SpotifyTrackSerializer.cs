using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Common;
using Pjfm.Domain.Enums;

namespace Pjfm.Infrastructure.Service
{
    public class SpotifyTrackSerializer
    {
        public List<TrackDto> ConvertMultiple(string jsonString)
        {
            dynamic objectResult = SerializeJson(jsonString);

            List<TrackDto> topTracks = new List<TrackDto>();
            
            foreach (var track in objectResult.tracks.items)
            {
                topTracks.Add(SerializeToTrack(track));
            }

            return topTracks;
        }

        public TrackDto ConvertSingle(string jsonString)
        {
            var objectResult = SerializeJson(jsonString);

            return SerializeToTrack(objectResult);
        }
        
        private JObject SerializeJson(string jsonString)
        {
            JObject objectResult = JsonConvert.DeserializeObject<dynamic>(jsonString, new JsonSerializerSettings()
            {
                ContractResolver = new UnderScorePropertyNamesContractResolver()
            });
            return objectResult;
        }
        
        private TrackDto SerializeToTrack(dynamic track)
        {
            List<string> artistNames = new List<string>();

            foreach (var artist in track.artists)
            {
                artistNames.Add((string) artist.name);
            }

            return new TrackDto
            {
                Title = track.name,
                Artists = artistNames.ToArray(),
                MainArtistId = track.artists[0].id,
                TrackType = TrackType.RequestedTrack,
                Id = track.id,
                SongDurationMs = track.duration_ms,
            };
        }
    }
}