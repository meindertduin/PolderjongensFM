﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pjfm.Application.Common;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Common;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Enums;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Spotify.Commands
{
    public class UpdateUserTopTracksCommand : IRequestWrapper<string>
    {
        public string AccessToken { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class UpdateUserTopTracksCommandHandler : IHandlerWrapper<UpdateUserTopTracksCommand, string>
    {
        private readonly IAppDbContext _ctx;
        private readonly ISpotifyTopTracksClient _spotifyTopTracksClient;

        public UpdateUserTopTracksCommandHandler(IAppDbContext ctx, ISpotifyTopTracksClient spotifyTopTracksClient)
        {
            _ctx = ctx;
            _spotifyTopTracksClient = spotifyTopTracksClient;
        }
        
        public async Task<Response<string>> Handle(UpdateUserTopTracksCommand request, CancellationToken cancellationToken)
        {
            var terms = SetTermsToUpdate(request);

            foreach (var term in terms)
            {
                var termTopTracks = _ctx.ApplicationUsers
                    .Where(u => u.Id == request.User.Id)
                    .SelectMany(x => x.TopTracks.Where(t => t.Term == (TopTrackTerm) term))
                    .ToList();

                termTopTracks = await _spotifyTopTracksClient.GetTopTracks(request.AccessToken, term, request.User.Id);
                await _ctx.SaveChangesAsync(cancellationToken);
            }
            
            return Response.Ok("succeeded", "toptracks have been saved to the database");
        }

        private Queue<int> SetTermsToUpdate(UpdateUserTopTracksCommand request)
        {
            var userTopTracks = _ctx.ApplicationUsers
                .Where(u => u.Id == request.User.Id)
                .Select(u => new
                {
                    ShortTermTracksExpired = u.TopTracks
                        .Any(x => x.Term == TopTrackTerm.ShortTerm
                                  && x.TimeAdded < DateTime.Now.AddDays(-28)),

                    MediumTermTracksExpired = u.TopTracks
                        .Any(x => x.Term == TopTrackTerm.ShortTerm
                                  && x.TimeAdded < DateTime.Now.AddDays(-28)),

                    LongTermTracksExpired = u.TopTracks
                        .Any(x => x.Term == TopTrackTerm.ShortTerm
                                  && x.TimeAdded < DateTime.Now.AddDays(-28)),
                }).FirstOrDefault();

            Queue<int> terms = new Queue<int>();


            Debug.Assert(userTopTracks != null, nameof(userTopTracks) + " != null");
            if (userTopTracks.ShortTermTracksExpired)
            {
                terms.Enqueue(0);
            }

            if (userTopTracks.MediumTermTracksExpired)
            {
                terms.Enqueue(1);
            }

            if (userTopTracks.LongTermTracksExpired)
            {
                terms.Enqueue(2);
            }

            return terms;
        }
    }
}