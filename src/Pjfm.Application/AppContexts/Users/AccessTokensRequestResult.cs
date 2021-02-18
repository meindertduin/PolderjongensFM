namespace Pjfm.Application.Spotify.Commands
{
    public class AccessTokensRequestResult
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
}