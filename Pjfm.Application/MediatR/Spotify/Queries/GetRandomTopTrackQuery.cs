using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Spotify.Queries
{
    public class GetRandomTopTrackQuery : IRequestWrapper<List<TopTrack>>
    {
        public List<TopTrack> NotIncludeTracks { get; set; }
        public int RequestedAmount { get; set; }
    }

    public class GetRandomTopTrackQueryHandler : IHandlerWrapper<GetRandomTopTrackQuery, List<TopTrack>>
    {
        private readonly IAppDbContext _ctx;

        public GetRandomTopTrackQueryHandler(IAppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public Task<Response<List<TopTrack>>> Handle(GetRandomTopTrackQuery request, CancellationToken cancellationToken)
        {
            var randomTopTracks = _ctx.TopTracks
                .Where(x => request.NotIncludeTracks.Select(t => t.Id).Contains(x.Id) == false)
                .OrderBy(x => Guid.NewGuid())
                .Take(request.RequestedAmount)
                .ToList();

            return Task.FromResult(Response.Ok("successfully queried result", randomTopTracks));
        }
    }
}