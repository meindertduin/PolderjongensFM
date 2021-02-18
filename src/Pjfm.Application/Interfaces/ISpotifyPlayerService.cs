using System.Net.Http;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;

namespace Pjfm.Domain.Interfaces
{
    public interface ISpotifyPlayerService
    {
        Task<HttpResponseMessage> Play(string userId, string accessToken, string deviceId,
            PlayRequestDto playRequestDto = null);

        Task<HttpResponseMessage> AddTrackToQueue(string userId, string accessToken, string trackId,
            string deviceId = null);

        Task<HttpResponseMessage> SkipSong(string userId, string accessToken, string deviceId = null);

        Task<HttpResponseMessage> PausePlayer(string userId, string accessToken, string deviceId = null);

        Task<HttpResponseMessage> GetDevices(string userId, string accessToken);
    }
}