﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Common.Mediatr;
using Pjfm.Application.Common.Mediatr.Wrappers;
using Pjfm.Domain.Entities;

namespace Pjfm.Application.AppContexts.Test.Queries
{
    public class GetUserTopTracksQuery : IRequestWrapper<List<TopTrack>>
    {
        public string UserID { get; set; }
    }
    
    public class GetUserTopTracksQueryHandler : IHandlerWrapper<GetUserTopTracksQuery, List<TopTrack>>
    {
        private readonly IAppDbContext _ctx;

        public GetUserTopTracksQueryHandler(IAppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public Task<Response<List<TopTrack>>> Handle(GetUserTopTracksQuery request, CancellationToken cancellationToken)
        {
            var userTopTracks = _ctx.TopTracks
                .AsNoTracking()
                .Where(x => x.ApplicationUserId == request.UserID)
                .ToList();

            return Task.FromResult(Response.Ok("user top tracks were succesfully queries", userTopTracks));
        }
    }
}