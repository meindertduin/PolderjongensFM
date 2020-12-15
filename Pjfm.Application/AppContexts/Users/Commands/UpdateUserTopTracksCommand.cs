using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Application.Services;
using Pjfm.Domain.Common;
using Pjfm.Domain.Converters;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Enums;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Spotify.Commands
{
    public class UpdateUserTopTracksCommand : IRequestWrapper<string>
    {
        public ApplicationUser User { get; set; }
    }

    public class UpdateUserTopTracksCommandHandler : IHandlerWrapper<UpdateUserTopTracksCommand, string>
    {
        private readonly IAppDbContext _ctx;
        private readonly IRetrieveStrategy _retrieveStrategy;
        private readonly ISpotifyBrowserService _spotifyBrowserService;

        public UpdateUserTopTracksCommandHandler(IAppDbContext ctx, IRetrieveStrategy retrieveStrategy, 
            ISpotifyBrowserService spotifyBrowserService)
        {
            _ctx = ctx;
            _retrieveStrategy = retrieveStrategy;
            _spotifyBrowserService = spotifyBrowserService;
        }
        
        public async Task<Response<string>> Handle(UpdateUserTopTracksCommand request, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(request.User.SpotifyRefreshToken))
            {
                return Response.Fail<string>("User has no refresh token");
            }

            List<TopTrack> updatedTopTracks = new List<TopTrack>();
            
            for (int i = 0; i < 3; i++)
            {
                var newTopTracksResult =
                    await _spotifyBrowserService.GetUserTopTracks(request.User.Id, request.User.SpotifyAccessToken, i);

                if (newTopTracksResult.IsSuccessStatusCode)
                {
                    var jsonData = await newTopTracksResult.Content.ReadAsStringAsync();
                    JObject objectResult = JsonConvert.DeserializeObject<dynamic>(jsonData, new JsonSerializerSettings()
                    {
                        ContractResolver = new UnderScorePropertyNamesContractResolver()
                    });

                    var topTracksMapper = new TopTracksMapper();
                    updatedTopTracks.AddRange(topTracksMapper.MapTopTrackItems(objectResult, i, request.User.Id));
                }
            }
            
            var termTopTracks = _ctx.ApplicationUsers
                .Where(u => u.Id == request.User.Id)
                .SelectMany(x => x.TopTracks)
                .ToArray();

            if (termTopTracks.Length <= 0)
            {
                await _ctx.TopTracks.AddRangeAsync(updatedTopTracks, cancellationToken);
            }
            else
            {
                for (int i = 0; i < updatedTopTracks.Count; i++)
                {
                    foreach (var termTopTrack in termTopTracks)
                    {
                        termTopTrack.Artists = updatedTopTracks[i].Artists;
                        termTopTrack.Term = updatedTopTracks[i].Term;
                        termTopTrack.Title = updatedTopTracks[i].Title;
                        termTopTrack.TimeAdded = updatedTopTracks[i].TimeAdded;
                        termTopTrack.SongDurationMs = updatedTopTracks[i].SongDurationMs;
                    }
                }
            }
            
            await _ctx.SaveChangesAsync(cancellationToken);

            return Response.Ok("succeeded", "topt racks have been saved to the database");
        }
    }
}