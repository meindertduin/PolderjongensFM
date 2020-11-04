using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Pjfm.Application.Common;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Enums;
using Pjfm.Domain.Interfaces;
using Pjfm.WebClient.Services;

namespace Pjfm.Application.Spotify.Queries
{
    public class GetRandomTopTrackQuery : IRequestWrapper<List<TrackDto>>
    {
        public List<TrackDto> NotIncludeTracks { get; set; }
        public int RequestedAmount { get; set; }
        public List<TopTrackTerm> TopTrackTermFilter { get; set; }
    }

    public class GetRandomTopTrackQueryHandler : IHandlerWrapper<GetRandomTopTrackQuery, List<TrackDto>>
    {
        private readonly IAppDbContext _ctx;

        public GetRandomTopTrackQueryHandler(IAppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public async Task<Response<List<TrackDto>>> Handle(GetRandomTopTrackQuery request, CancellationToken cancellationToken)
        {
            var randomTopTracks = _ctx.TopTracks
                .AsNoTracking()
                .Where(x => request.NotIncludeTracks.Select(t => t.Id).Contains(x.Id) == false)
                .Where(x => request.TopTrackTermFilter.Select(t => t).Contains(x.Term))
                .OrderBy(x => Guid.NewGuid())
                .Take(request.RequestedAmount)
                .ProjectTo<TrackDto>(new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TopTrack, TrackDto>().BeforeMap((s, d) => d.TrackType = TrackType.UserTopTrack);
                }))
                .ToList();

            return Response.Ok("successfully queried result", randomTopTracks);
        }
    }
}