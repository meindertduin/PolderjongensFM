using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Enums;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Spotify.Queries
{
    public class GetRandomTopTrackQuery : IRequestWrapper<List<TrackDto>>
    {
        public List<TrackDto> NotIncludeTracks { get; set; }
        public int RequestedAmount { get; set; }
        public List<TopTrackTerm> TopTrackTermFilter { get; set; }
        public string[] IncludedUsersId { get; set; }
    }

    public class GetRandomTopTrackQueryHandler : IHandlerWrapper<GetRandomTopTrackQuery, List<TrackDto>>
    {
        private readonly IAppDbContext _appDbContext;

        public GetRandomTopTrackQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Task<Response<List<TrackDto>>> Handle(GetRandomTopTrackQuery request,
            CancellationToken cancellationToken)
        {
            var tracks = _appDbContext.TopTracks
                .OrderBy(_ => Guid.NewGuid())
                .Take(request.RequestedAmount)
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

            return Task.FromResult(Response.Ok("queried tracks successfully", tracks));
        }
    }
}