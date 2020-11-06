using System;
using System.Collections.Generic;
using Pjfm.Application.Common.Dto;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackController
    {
        static IPlaybackState CurrentPlaybackState { get; protected set; }
        void SetPlaybackState(IPlaybackState state);
        void TurnOn(PlaybackControllerCommands command);
        void TurnOff(PlaybackControllerCommands command);
        void Undo();
        void AddPriorityTrack(TrackDto track);
        void AddSecondaryTrack(TrackDto track);
        List<TrackDto> GetPriorityQueueTracks();
        List<TrackDto> GetFillerQueueTracks();
        Tuple<TrackDto, DateTime> GetPlayingTrackInfo();
        void SetUsersInclusionList(List<ApplicationUserDto> users)
        IDisposable SubscribeToPlayingStatus(IObserver<bool> observer);
    }
}