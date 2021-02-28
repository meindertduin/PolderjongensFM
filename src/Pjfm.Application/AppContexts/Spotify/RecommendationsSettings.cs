using System;

namespace Pjfm.Application.AppContexts.Spotify
{
    public class RecommendationsSettings
    {
        public int Limit { get; set; }
        public string SeedArtists { get; set; }
        public string SeedGenres { get; set; }
        public string SeedTracks { get; set; }
    }
}