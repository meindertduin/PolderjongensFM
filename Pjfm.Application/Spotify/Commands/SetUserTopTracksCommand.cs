using System;
using System.Collections.Generic;
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
    public class SetUserTopTracksCommand : IRequestWrapper<string>
    {
        public string AccessToken { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class SetUserTopTracksCommandHandler : IHandlerWrapper<SetUserTopTracksCommand, string>
    {
        private readonly IAppDbContext _ctx;
        private readonly ISpotifyTopTracksClient _spotifyTopTracksClient;

        public SetUserTopTracksCommandHandler(IAppDbContext ctx, ISpotifyTopTracksClient spotifyTopTracksClient)
        {
            _ctx = ctx;
            _spotifyTopTracksClient = spotifyTopTracksClient;
        }
        
        public async Task<Response<string>> Handle(SetUserTopTracksCommand request, CancellationToken cancellationToken)
        {
            List<TopTrack> topTracks = new List<TopTrack>();

            for (int i = 0; i < 3; i++)
            {
                var termTopTracks = await _spotifyTopTracksClient.GetTopTracks(request.AccessToken, i, request.User.Id);
                topTracks.AddRange(termTopTracks);
            }

            await _ctx.TopTracks.AddRangeAsync(topTracks, cancellationToken);

            return Response.Ok("succeeded", "toptracks have been saved to the database");
        }
    }
}