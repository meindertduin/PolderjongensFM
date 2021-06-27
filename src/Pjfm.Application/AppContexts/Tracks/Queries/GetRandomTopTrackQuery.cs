using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Enums;
using Pjfm.Domain.Interfaces;

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
        private readonly IConfiguration _configuration;
        private readonly IAppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public GetRandomTopTrackQueryHandler(IConfiguration configuration, IAppDbContext appDbContext, IMapper mapper)
        {
            _configuration = configuration;
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<Response<List<TrackDto>>> Handle(GetRandomTopTrackQuery request,
            CancellationToken cancellationToken)
        {
            var tracksResult = _appDbContext.TopTracks
                .OrderBy(_ => Guid.NewGuid())
                .Take(request.RequestedAmount)
                .AsNoTracking()
                .ToList();

            var tracks = _mapper.Map<List<TrackDto>>(tracksResult);
            
            return Response.Ok("queried tracks successfully", tracks);
        }
    }
}