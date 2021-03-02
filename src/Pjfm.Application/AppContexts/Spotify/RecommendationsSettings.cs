using System;

namespace Pjfm.Application.AppContexts.Spotify
{
    public class RecommendationsSettings
    {
        // Warning: Do not rename these values as the name is important for parsing them to spotify Api requests
        public int Limit { get; set; }
        public string SeedArtists { get; set; }
        public string SeedGenres { get; set; }
        public string SeedTracks { get; set; }
        public int? MinPopularity { get; set; }
        public int? MaxPopularity { get; set; }
        public int? TargetPopularity { get; set; }
        public int? MinTempo { get; set; }
        public int? MaxTempo { get; set; }
        public int? TargetTempo { get; set; }
        public decimal? MinInstrumentalness { get; set; }
        public decimal? MaxInstrumentalness { get; set; }
        public decimal? TargetInstrumentalness { get; set; }
        public decimal? MinEnergy { get; set; }
        public decimal? MaxEnergy { get; set; }
        public decimal? TargetEnergy { get; set; }
        public decimal? MinDanceability { get; set; }
        public decimal? MaxDanceability { get; set; }
        public decimal? TargetDanceability { get; set; }
    }
}