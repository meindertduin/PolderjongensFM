using System.Collections.Generic;
using System.Linq;
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
        private readonly PlaybackQueue _playbackQueue;
        private readonly IMediator _mediator;

        public UsersTopTracksFillerQueueState(PlaybackQueue playbackQueue, IMediator mediator)
        {
            _playbackQueue = playbackQueue;
            _mediator = mediator;
        }
        
        public void AddRecentlyPlayed(TrackDto track)
        {
            RecentlyPlayed.Add(track);    
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