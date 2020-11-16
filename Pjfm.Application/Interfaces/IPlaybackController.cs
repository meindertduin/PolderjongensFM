using System;
using System.Collections.Generic;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using pjfm.Models;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackController
    {
        static IPlaybackState CurrentPlaybackState { get; protected set; }
        void SetPlaybackState(IPlaybackState state);
        void TurnOn(PlaybackControllerCommands command);
        void TurnOff(PlaybackControllerCommands command);
        void Undo();
        List<ApplicationUserDto> GetIncludedUsers();
        void AddIncludedUser(ApplicationUserDto user);
        void RemoveIncludedUser(ApplicationUserDto user);
        Response<bool> AddPriorityTrack(TrackDto track);
        Response<bool> AddSecondaryTrack(TrackDto track, string userId);
        List<TrackDto> GetPriorityQueueTracks();
        List<TrackDto> GetSecondaryQueueTracks();
        List<TrackDto> GetFillerQueueTracks();
        Tuple<TrackDto, DateTime> GetPlayingTrackInfo();
        PlaybackSettingsDto GetPlaybackSettings();
        IDisposable SubscribeToPlayingStatus(IObserver<bool> observer);
        
    }
}