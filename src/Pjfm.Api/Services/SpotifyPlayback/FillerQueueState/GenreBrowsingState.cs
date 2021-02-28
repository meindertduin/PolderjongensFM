using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.WebClient.Services;
using Pjfm.WebClient.Services.FillerQueueState;

namespace Pjfm.Api.Services.SpotifyPlayback.FillerQueueState
{
    public class GenreBrowsingState : FillerQueueStateBase, IFillerQueueState
    {
        private readonly PlaybackQueue _playbackQueue;
        private readonly IMediator _mediator;

        public GenreBrowsingState(PlaybackQueue playbackQueue, IMediator mediator)
        {
            _playbackQueue = playbackQueue;
            _mediator = mediator;
        }
        public Task<Response<List<TrackDto>>> RetrieveFillerTracks(int amount)
        {
            var settings = _playbackQueue.PlaybackQueueSettings;
            return Task.FromResult(Response.Ok("retrieved tracks succesfully", new List<TrackDto>()));
        }
    }
}