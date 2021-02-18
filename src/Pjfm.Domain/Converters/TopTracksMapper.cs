using System;
using System.Collections.Generic;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Enums;
using Pjfm.Domain.ValueObjects;

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

                string title = item.name.ToString();

                topTracksResult.Add(new TopTrack
                {
                    SpotifyTrackId = item.id,
                    Title = title.WithMaxLength(100),
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