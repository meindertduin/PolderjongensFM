using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using pjfm.Models;
using Pjfm.WebClient.Services.FillerQueueState;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackController
    {
        static IPlaybackState CurrentPlaybackState { get; protected set; }
        void SetPlaybackState(IPlaybackState state);
        void SetMaxRequestsPerUserAmount(int amount);
        void TurnOn(PlaybackControllerCommands command);
        void TurnOff(PlaybackControllerCommands command);
        void Undo();
        Task SynchWithPlayback(string userId, string spotifyAccessToken, PlaybackDevice playbackDevice);
        List<ApplicationUserDto> GetIncludedUsers();
        void AddIncludedUser(ApplicationUserDto user);
        bool TryRemoveIncludedUser(ApplicationUserDto user);
        void SetFillerQueueState(FillerQueueType fillerQueueType);
        void SetBrowserQueueSettings(BrowserQueueSettings settings);
        void DequeueTrack(string trackId);
        Response<bool> AddPriorityTrack(TrackDto track);
        Response<bool> AddSecondaryTrack(TrackDto track, ApplicationUserDto user);
        List<TrackDto> GetPriorityQueueTracks();
        List<TrackDto> GetSecondaryQueueTracks();
        List<TrackDto> GetFillerQueueTracks();
        Tuple<TrackDto, DateTime> GetPlayingTrackInfo();
        PlaybackSettingsDto GetPlaybackSettings();
        bool IsPlaying();
        IDisposable SubscribeToPlayingStatus(IObserver<bool> observer);
    }
}