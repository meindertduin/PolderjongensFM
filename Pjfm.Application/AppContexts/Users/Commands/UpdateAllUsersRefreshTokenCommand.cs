using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Spotify.Commands
{
    public class UpdateAllUsersRefreshTokenCommand : IRequestWrapper<string>
    {
        
    }

    public class UpdateAllUsersRefreshTokenCommandHandler : IHandlerWrapper<UpdateAllUsersRefreshTokenCommand, string>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IMediator _mediator;

        public UpdateAllUsersRefreshTokenCommandHandler(IAppDbContext appDbContext, IMediator mediator)
        {
            _appDbContext = appDbContext;
            _mediator = mediator;
        }
        
        public async Task<Response<string>> Handle(UpdateAllUsersRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            
            var userIds = _appDbContext.ApplicationUsers
                .AsNoTracking()
                .Where(user => String.IsNullOrEmpty(user.SpotifyRefreshToken) == false)
                .Select(x => x.Id)
                .ToArray();
            
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