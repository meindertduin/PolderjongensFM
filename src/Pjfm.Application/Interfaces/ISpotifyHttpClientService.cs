using System.Net.Http;
using System.Threading.Tasks;

namespace Pjfm.Domain.Interfaces
{
    public interface ISpotifyHttpClientService
    {
        Task<HttpResponseMessage> SendAuthenticatedRequest(HttpRequestMessage requestMessage, string userId,
            string accessToken);
    }
}