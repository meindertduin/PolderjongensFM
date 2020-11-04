namespace Pjfm.Application.Common.Dto
{
    public class RecommendationsRequestDto
    {
        public float Acousticness { get; set; }
        public float Danceability { get; set; }
        public int DurationMs { get; set; }
        public float Energy { get; set; }
        public float Instrumentalness { get; set; }
        public int Key { get; set; }
        public float Liveness { get; set; }
        public float Loudness { get; set; }
        public int Mode { get; set; }
        public int Popularity { get; set; }
        public float Speechiness { get; set; }
        public float Tempo { get; set; }
        public int TimeSignature { get; set; }
        public float Valence { get; set; }
    }
}