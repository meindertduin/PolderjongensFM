using System.Net.Http;
using System.Threading.Tasks;

namespace Pjfm.Application.Interfaces
{
    public interface ISpotifyHttpClientService
    {
        Task<HttpResponseMessage> SendAccessTokenRequest(HttpRequestMessage requestMessage, string userId,
            string accessToken);
        Task<HttpResponseMessage> SendClientCredentialsRequest(HttpRequestMessage requestMessage);
    }
}