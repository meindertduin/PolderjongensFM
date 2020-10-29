namespace Pjfm.Application.Spotify
{
    public class RefreshAccessTokenRequestResult
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string Scope { get; set; }
        public string ExpiresIn { get; set; }
    }
}