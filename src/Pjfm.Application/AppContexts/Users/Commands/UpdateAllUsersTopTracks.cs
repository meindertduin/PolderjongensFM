using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Common.Mediatr;
using Pjfm.Application.Common.Mediatr.Wrappers;

namespace Pjfm.Application.AppContexts.Users.Commands
{
    /// <summary>
    /// used by mediatr to handle refreshing all users spotify toptracks
    /// </summary>
    public class UpdateAllUsersTopTracks : IRequestWrapper<string>
    {
        
    }

    public class UpdateAllUsersRefreshTokenCommandHandler : IHandlerWrapper<UpdateAllUsersTopTracks, string>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IMediator _mediator;

        public UpdateAllUsersRefreshTokenCommandHandler(IAppDbContext appDbContext, IMediator mediator)
        {
            _appDbContext = appDbContext;
            _mediator = mediator;
        }
        
        public async Task<Response<string>> Handle(UpdateAllUsersTopTracks request, CancellationToken cancellationToken)
        {
            // get userId's of all users which contain a spotify refresh token and are spotify authenticated
            var userIds = _appDbContext.ApplicationUsers
                .AsNoTracking()
                .Where(user => String.IsNullOrEmpty(user.SpotifyRefreshToken) == false && user.SpotifyAuthenticated)
                .Select(x => x.Id)
                .ToArray();
            
            // update topTracks for all userId's
            foreach (var userId in userIds)
            {
                var updateTask = _mediator.Send(new UpdateUserTopTracksCommand()
                {
                    UserId = userId,
                }, cancellationToken);
                await updateTask;
            }


            return Response.Ok("RefreshTokens of users updated", String.Empty);
        }
    }
}