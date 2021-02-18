using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Application.Services;
using Pjfm.Domain.Common;
using Pjfm.Domain.Converters;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Interfaces;
using Pjfm.Domain.ValueObjects;
using Serilog;

namespace Pjfm.Application.Spotify.Commands
{
    /// <summary>
    /// used by mediatr to handle updating a user's topTracks with the spotify api
    /// </summary>
    public class UpdateUserTopTracksCommand : IRequestWrapper<string>
    {
        public string UserId { get; set; }
    }

    public class UpdateUserTopTracksCommandHandler : IHandlerWrapper<UpdateUserTopTracksCommand, string>
    {
        private readonly IAppDbContext _ctx;
        private readonly ISpotifyBrowserService _spotifyBrowserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int TopTracksRetrievalCount = 150;

        public UpdateUserTopTracksCommandHandler(IAppDbContext ctx, ISpotifyBrowserService spotifyBrowserService, UserManager<ApplicationUser> userManager)
        {
            _ctx = ctx;
            _spotifyBrowserService = spotifyBrowserService;
            _userManager = userManager;
        }
        
        public async Task<Response<string>> Handle(UpdateUserTopTracksCommand request, CancellationToken cancellationToken)
        {
            Log.Information("updating user toptracks");
            var user = _ctx.ApplicationUsers.AsNoTracking().FirstOrDefault(x => x.Id == request.UserId);
            
            if (user == null || String.IsNullOrEmpty(user.SpotifyRefreshToken))
            {
                return Response.Fail<string>("User has no refresh token");
            }

            try
            {
                 List<TopTrack> updatedTopTracks = new List<TopTrack>();
                 
                 // iterate three times to get get tracks of terms short, medium and long
                for (int i = 0; i < 3; i++)
                {
                    // get topTracks of term
                    var newTopTracksResult =
                        await _spotifyBrowserService.GetUserTopTracks(user.Id, user.SpotifyAccessToken, i);

                    if (newTopTracksResult.IsSuccessStatusCode)
                    {
                        var jsonData = await newTopTracksResult.Content.ReadAsStringAsync();
                        JObject objectResult = JsonConvert.DeserializeObject<dynamic>(jsonData, new JsonSerializerSettings()
                        {
                            ContractResolver = new UnderScorePropertyNamesContractResolver()
                        });

                        // map json data to topTrack objects and add them to updatedTopTracks
                        var topTracksMapper = new TopTracksMapper();
                        updatedTopTracks.AddRange(topTracksMapper.MapTopTrackItems(objectResult, i, user.Id));
                    }
                }
                
                var termTopTracks = _ctx.ApplicationUsers
                    .Where(u => u.Id == user.Id)
                    .SelectMany(x => x.TopTracks)
                    .ToArray();

                // add updatedTopTracks as range if user has no topTracks yet
                if (termTopTracks.Length <= 0)
                {
                    await _ctx.TopTracks.AddRangeAsync(updatedTopTracks, cancellationToken);
                }
                // update existing topTracks
                else if(updatedTopTracks.Count > 0)
                {
                    _ctx.TopTracks.RemoveRange(termTopTracks);
                    await _ctx.TopTracks.AddRangeAsync(updatedTopTracks, cancellationToken);
                }
                
                await _ctx.SaveChangesAsync(cancellationToken);

                return Response.Ok("succeeded", "topt racks have been saved to the database");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return Response.Fail(e.Message, String.Empty);
            }
        }
    }
}