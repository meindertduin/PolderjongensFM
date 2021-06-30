using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Interfaces;
using Pjfm.WebClient.Services;
using Pjfm.WebClient.Services.FillerQueueState;

namespace Pjfm.Api.Services.SpotifyPlayback.FillerQueueState
{
    public class UsersTopTracksFillerQueueState: FillerQueueStateBase, IFillerQueueState
    {
        private readonly PlaybackQueue _playbackQueue;
        private readonly IAppDbContext _appDbContext;

        public UsersTopTracksFillerQueueState(PlaybackQueue playbackQueue, IAppDbContext appDbContext)
        {
            _playbackQueue = playbackQueue;
            _appDbContext = appDbContext;
        }
        
        public Task<List<TrackDto>> RetrieveFillerTracks(int amount)
        {
            var settings = _playbackQueue.PlaybackQueueSettings;

            var topTrackTermFilter = settings.TopTrackTermFilter.ConvertToTopTrackTerms();
            var includedUsersIds = settings.IncludedUsers.Select(user => user.Id).ToArray();
            
            // TODO: fix this!
            // This is very ugly and goes against the whole arhitecure of the application. But because
            // sending mediator requests from here was causing errors, retrieving tracks is done here (hopefully) temparaily
            var tracks = _appDbContext.TopTracks
                .OrderBy(_ => Guid.NewGuid())
                .Where(t => RecentlyPlayed.Select(r => r.Id).Contains(t.SpotifyTrackId) == false)
                .Where(t => topTrackTermFilter.Select(f => f).Contains(t.Term))
                .Where(t => includedUsersIds.Length <= 0 || includedUsersIds.Select(i => i).Contains(t.ApplicationUserId))
                .Take(amount)
                .Select(track => new TrackDto()
                {
                    Id = track.SpotifyTrackId,
                    Artists = track.Artists,
                    Term = track.Term,
                    Title = track.Title,
                    SongDurationMs = track.SongDurationMs,
                    User = new ApplicationUserDto()
                    {
                        DisplayName = track.ApplicationUser.DisplayName,
                        Id = track.ApplicationUser.Id,
                        Member = track.ApplicationUser.Member,
                        SpotifyAuthenticated = track.ApplicationUser.SpotifyAuthenticated,
                    }
                })
                .AsNoTracking()
                .ToList();
            
            return Task.FromResult(tracks);
        }

    }
}