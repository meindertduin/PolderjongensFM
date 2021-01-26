using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Pjfm.Application.AppContexts.Tracks;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Enums;

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

        public GetRandomTopTrackQueryHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Response<List<TrackDto>>> Handle(GetRandomTopTrackQuery request,
            CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_configuration["ConnectionStrings:ApplicationDb"]))
            {
                var result = await connection.QueryAsync<dynamic>(
                    GenerateRandomTracksSqlQuery(request.IncludedUsersId.Length > 0), new
                {
                    TrackIds = request.NotIncludeTracks.Count > 0? request.NotIncludeTracks.Select(x => x.Id) : new [] { "" },
                    Terms = request.TopTrackTermFilter,
                    RequestedAmount = request.RequestedAmount,
                    UserIds = request.IncludedUsersId
                });

                var tracks = new List<TrackDto>();

                foreach (var queryEntity in result)
                {
                    tracks.Add(new TrackDto()
                    {
                        Id = queryEntity.Id,
                        Artists = queryEntity.Artists.Split(','),
                        Term = (TopTrackTerm) queryEntity.Term,
                        Title = queryEntity.Title,
                        SongDurationMs = queryEntity.SongDurationMs,
                        User = new ApplicationUserDto()
                        {
                            DisplayName = queryEntity.DisplayName,
                            Id = queryEntity.UserId,
                            Member = queryEntity.Member,
                            SpotifyAuthenticated = queryEntity.SpotifyAuthenticated,
                        }
                    });
                }

                return Response.Ok("queried tracks successfully", tracks);
            }
        }
        
        private string GenerateRandomTracksSqlQuery(bool withIncludedUsers)
        {
            var sqlBuilder = new StringBuilder(
                "SELECT TopTracks.Title, TopTracks.Artists, TopTracks.Term, TopTracks.SpotifyTrackId As Id, " +
                "TopTracks.SongDurationMs, AspNetUsers.Id AS UserId, AspNetUsers.DisplayName, AspNetUsers.Member, AspNetUsers.SpotifyAuthenticated ");
            
            sqlBuilder.Append("FROM PJFM.TopTracks ");
            sqlBuilder.Append("LEFT JOIN PJFM.AspNetUsers ");
            sqlBuilder.Append("ON TopTracks.ApplicationUserId = AspNetUsers.Id ");

            sqlBuilder.Append("WHERE TopTracks.Id NOT IN @TrackIds ");
            sqlBuilder.Append("And TopTracks.Term IN @Terms ");
            
            if (withIncludedUsers)
            {
                sqlBuilder.Append("And TopTracks.ApplicationUserId IN @UserIds ");
            }
            
            sqlBuilder.Append("ORDER BY rand() limit @RequestedAmount;");

            return sqlBuilder.ToString();
        }
    }
}