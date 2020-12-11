using System;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.Domain.Enums;

namespace Pjfm.WebClient.Services
{
    public class DefaultPlaybackState : IPlaybackState
    {
        private readonly IPlaybackQueue _playbackQueue;

        public DefaultPlaybackState(IPlaybackQueue playbackQueue)
        {
            _playbackQueue = playbackQueue;
        }

        public Response<bool> AddPriorityTrack(TrackDto track)
        {
            track.TrackType = TrackType.DjTrack;
            _playbackQueue.AddPriorityTrack(track);
            
            return Response.Ok("Nummer toegevoegd aan de wachtrij", true);
        }

        public Response<bool> AddSecondaryTrack(TrackDto track, ApplicationUserDto user)
        {
            throw new InvalidOperationException("Cant add secondary while in Default playback state");
        }
    }
}