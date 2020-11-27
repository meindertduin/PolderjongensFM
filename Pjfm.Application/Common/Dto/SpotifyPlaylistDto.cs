using System.Collections.Generic;

namespace Pjfm.Application.Common.Dto
{
    public class SpotifyPlaylistDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Href { get; set; }
        public string[] ExternalUrls { get; set; }
        public List<SpotifyPlaylistImage> Images { get; set; }
        public bool Collaborative { get; set; }
    }
}