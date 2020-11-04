using System;
using System.Collections.Generic;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Entities;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackController
    {
        void TurnOn(PlaybackControllerCommands command);
        void TurnOff(PlaybackControllerCommands command);
        void Undo();
        void AddPriorityTrack(TrackDto track);
        List<TrackDto> GetPriorityQueueTracks();
        List<TrackDto> GetFillerQueueTracks();
        Tuple<TrackDto, DateTime> GetPlayingTrackInfo();
        IDisposable SubscribeToPlayingStatus(IObserver<bool> observer);
    }
}