﻿using System.Net.Http;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;

namespace Pjfm.Domain.Interfaces
{
    public interface ISpotifyPlayerService
    {
        Task<HttpResponseMessage> Play(string userId, string accessToken, string deviceId,
            PlayRequestDto playRequestDto = null);
    }
}