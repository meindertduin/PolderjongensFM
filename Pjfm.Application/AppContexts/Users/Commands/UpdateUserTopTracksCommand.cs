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
using Serilog;

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
        private const int TopTracksRetrievalCount = 150;

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
            else if(updatedTopTracks.Count == TopTracksRetrievalCount)
            {
                for (int i = 0; i < updatedTopTracks.Count; i++)
                {
                    termTopTracks[i].SpotifyTrackId = updatedTopTracks[i].SpotifyTrackId;
                    termTopTracks[i].Artists = updatedTopTracks[i].Artists;
                    termTopTracks[i].Term = updatedTopTracks[i].Term;
                    termTopTracks[i].Title = updatedTopTracks[i].Title;
                    termTopTracks[i].TimeAdded = updatedTopTracks[i].TimeAdded;
                    termTopTracks[i].SongDurationMs = updatedTopTracks[i].SongDurationMs;
                }
            }
            else
            {
                _ctx.TopTracks.RemoveRange(termTopTracks);
                Log.Error($@"Error while trying to update users toptracks of user with userId: {request.User.Id}. As a result the toptracks of user are deleted and have to be re-updated.");
            }
            
            await _ctx.SaveChangesAsync(cancellationToken);

            return Response.Ok("}succeeded", "topt racks have been saved to the database");
        }
    }
}