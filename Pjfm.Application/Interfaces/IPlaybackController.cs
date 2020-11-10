using System;
using System.Collections.Generic;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackController
    {
        static IPlaybackState CurrentPlaybackState { get; protected set; }
        void SetPlaybackState(IPlaybackState state);
        void TurnOn(PlaybackControllerCommands command);
        void TurnOff(PlaybackControllerCommands command);
        void Undo();
        Response<bool> AddPriorityTrack(TrackDto track);
        Response<bool> AddSecondaryTrack(TrackDto track);
        List<TrackDto> GetPriorityQueueTracks();
        List<TrackDto> GetFillerQueueTracks();
        Tuple<TrackDto, DateTime> GetPlayingTrackInfo();
        void SetUsersInclusionList(List<ApplicationUserDto> users);
        IDisposable SubscribeToPlayingStatus(IObserver<bool> observer);
    }
}