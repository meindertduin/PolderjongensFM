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
using Pjfm.Application.Identity;
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
        public string[] IncludedUsersId { get; set; }
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
            var random = new Random();
            
            var randomTopTracks = _ctx.TopTracks
                .Where(x => request.NotIncludeTracks.Select(t => t.Id).Contains(x.SpotifyTrackId) == false)
                .Where(x => request.TopTrackTermFilter.Select(t => t).Contains(x.Term))
                .Where(x => request.IncludedUsersId.Length <= 0 || request.IncludedUsersId.Select(t => t).Contains(x.ApplicationUserId))
                .Select(x => new TrackDto()
                {
                    Id = x.SpotifyTrackId,
                    Title = x.Title,
                    Artists = x.Artists,
                    Term = x.Term,
                    SongDurationMs = x.SongDurationMs,
                    User = new ApplicationUserDto()
                    {
                        Id = x.ApplicationUser.Id,
                        DisplayName = x.ApplicationUser.DisplayName,
                        Member = x.ApplicationUser.Member,
                    },
                })
                .ToList();

            // Temporary
            Shuffle(randomTopTracks);

            return Response.Ok("successfully queried result", randomTopTracks.Take(request.RequestedAmount).ToList());
        }
        
        private static void Shuffle<T>(IList<T> list)
        {
            var random = new Random();
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = random.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }
    }
}