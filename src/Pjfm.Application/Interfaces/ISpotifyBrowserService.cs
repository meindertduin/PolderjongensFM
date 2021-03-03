using System;
using System.Net.Http;
using System.Threading.Tasks;
using Pjfm.Application.AppContexts.Playback;
using Pjfm.Application.AppContexts.Playlists;
using Pjfm.Application.AppContexts.Spotify;

namespace Pjfm.Application.Interfaces
{
    public interface ISpotifyBrowserService
    {
        Task<HttpResponseMessage> GetUserTopTracks(string userId, string accessToken, int term);
        Task<HttpResponseMessage> Search(string userId, string accessToken, SearchRequestDto searchRequestInfo);
        Task<HttpResponseMessage> GetTrackInfo(string userId, string accessToken, string trackId);
        
        Task<HttpResponseMessage> Me(string userId, string accessToken);

        Task<HttpResponseMessage> GetUserPlaylists(string userId, string accessToken,
            PlaylistRequestDto playlistRequest);

        Task<HttpResponseMessage> GetPlaylistTracks(string userId, string accessToken,
            PlaylistTracksRequestDto playlistTracksRequestDto);
        
        Task<HttpResponseMessage> GetTopTracks(string userId, string accessToken,
            TopTracksRequestDto topTracksRequestDto);

        Task<HttpResponseMessage> CustomRequest(string userId, string accessToken, Uri nextUri);
        Task<HttpResponseMessage> GetRecommendations(RecommendationsSettings settings);
        Task<HttpResponseMessage> ServerGetMultipleTracks(string[] trackIds);
        Task<HttpResponseMessage> GetSpotifyGenres();
    }
}