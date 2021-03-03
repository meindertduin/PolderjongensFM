using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Pjfm.Application.AppContexts.Tracks;
using Pjfm.Application.AppContexts.Tracks.Queries;
using Pjfm.Application.Common.Mediatr;
using Pjfm.Domain.Enums;

namespace Pjfm.Api.Services.SpotifyPlayback.FillerQueueState
{
    public class UsersTopTracksFillerQueueState: FillerQueueStateBase, IFillerQueueState
    {
        private readonly PlaybackQueue _playbackQueue;
        private readonly IMediator _mediator;

        public UsersTopTracksFillerQueueState(PlaybackQueue playbackQueue, IMediator mediator)
        {
            _playbackQueue = playbackQueue;
            _mediator = mediator;
        }
        
        public Task<Response<List<TrackDto>>> RetrieveFillerTracks(int amount)
        {
            var settings = _playbackQueue.PlaybackQueueSettings;
            return _mediator.Send(new GetRandomTopTrackQuery()
                {
                    NotIncludeTracks = RecentlyPlayed,
                    RequestedAmount = amount,
                    TopTrackTermFilter = settings.TopTrackTermFilter.ConvertToTopTrackTerms(),
                    IncludedUsersId = settings.IncludedUsers.Select(user => user.Id).ToArray(),
                });
        }

    }
}