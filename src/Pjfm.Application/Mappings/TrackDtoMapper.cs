using System.Collections.Generic;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Enums;

namespace Pjfm.Application.Mappings
{
    public class TrackDtoMapper
    {
        public List<TrackDto> MapObjects(dynamic value)
        {
            var result = new List<TrackDto>();

            foreach (var track in value.tracks)
            {
                List<string> artistNames = new List<string>();
                
                foreach (var artist in track.artists)
                {
                    artistNames.Add((string) artist.name);
                }
                
                result.Add(new TrackDto()
                {
                    Id = track.id,
                    Title = track.name,
                    TrackType = TrackType.UserTopTrack,
                    MainArtistId = track.artists[0].id,
                    Artists = artistNames.ToArray(),
                    SongDurationMs = track.duration_ms,
                }); 
            }

            return result;
        }
    }
}