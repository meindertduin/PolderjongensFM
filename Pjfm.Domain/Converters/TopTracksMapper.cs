using System;
using System.Collections.Generic;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Enums;

namespace Pjfm.Domain.Converters
{
    public class TopTracksMapper
    {
        public List<TopTrack> MapTopTrackItems(dynamic topTrackJsonObject, int term, string userId)
        {
            List<TopTrack> topTracksResult = new List<TopTrack>();
            
            foreach (var item in topTrackJsonObject.items)
            {
                List<string> artistNames = new List<string>();

                foreach (var artist in item.artists)
                {
                    artistNames.Add((string) artist.name);
                }

                topTracksResult.Add(new TopTrack
                {
                    Id = item.id,
                    Title = item.name,
                    Artists = artistNames.ToArray(),
                    Term = (TopTrackTerm) term,
                    ApplicationUserId = userId,
                    TimeAdded = DateTime.Now,
                    SongDurationMs = item.duration_ms,
                });
            }

            return topTracksResult;
        }
    }
}