using System;

namespace Pjfm.Application.AppContexts.Spotify
{
    public class RecommendationsSettings
    {
        public int Limit { get; set; }
        public string SeedArtists { get; set; }
        public string SeedGenres { get; set; }
        public string SeedTracks { get; set; }
        public int? MinPopularity { get; set; }
        public int? MaxPopularity { get; set; }
        public int? TargetPopularity { get; set; }
        public decimal? MinInstrumentalness { get; set; }
        public decimal? MaxInstrumentalness { get; set; }
        public decimal? TargetInstrumentalness { get; set; }
    }
}