using System.Net.Http;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;

namespace Pjfm.Application.Services
{
    public interface ISpotifyBrowserService
    {
        Task<HttpResponseMessage> Search(string userId, string accessToken, SearchRequestDto searchRequestInfo);
    }
}