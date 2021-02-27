using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.Application.Spotify.Queries;
using Pjfm.Domain.Enums;

namespace Pjfm.WebClient.Services.FillerQueueState
{
    public class UsersTopTracksFillerQueueState: FillerQueueStateBase, IFillerQueueState
    {
        private readonly IMediator _mediator;

        public UsersTopTracksFillerQueueState(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public void AddRecentlyPlayed(TrackDto track)
        {
            RecentlyPlayed.Add(track);    
        }

        public Task<Response<List<TrackDto>>> RetrieveFillerTracks(int amount)
        {
            return _mediator.Send(new GetRandomTopTrackQuery()
                {
                    NotIncludeTracks = RecentlyPlayed,
                    RequestedAmount = amount,
                    TopTrackTermFilter = new List<TopTrackTerm> {  TopTrackTerm.LongTerm },
                    // Todo: change this in future, otherwise included users will not work
                    IncludedUsersId = new string[0],
                });
        }

    }
}