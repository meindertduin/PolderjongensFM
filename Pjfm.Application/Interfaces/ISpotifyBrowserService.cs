﻿using System.Net.Http;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;

namespace Pjfm.Application.Services
{
    public interface ISpotifyBrowserService
    {
        Task<HttpResponseMessage> Search(string userId, string accessToken, SearchRequestDto searchRequestInfo);
        Task<HttpResponseMessage> GetTrackInfo(string userId, string accessToken, string trackId);

        Task<HttpResponseMessage> GetUserPlaylists(string userId, string accessToken,
            PlaylistRequestDto playlistRequest);

        Task<HttpResponseMessage> GetPlaylistTracks(string userId, string accessToken,
            PlaylistTracksRequestDto playlistTracksRequestDto);
    }
}