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
        public string AccessToken { get; set; }
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
                
                    var termTopTracks = _ctx.ApplicationUsers
                        .Where(u => u.Id == request.User.Id)
                        .SelectMany(x => x.TopTracks.Where(t => t.Term == (TopTrackTerm) i))
                        .ToList();
                
                    termTopTracks = topTracksMapper.MapTopTrackItems(objectResult, i, request.User.Id);
                
                    await _ctx.SaveChangesAsync(cancellationToken);
                }
            }
            
            return Response.Ok("succeeded", "topt racks have been saved to the database");
        }
    }
}