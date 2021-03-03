using System;
using System.Threading.Tasks;
using Pjfm.Application.AppContexts.Playback;
using Pjfm.Application.AppContexts.Spotify;
using Pjfm.Application.AppContexts.Tracks;
using Pjfm.Application.AppContexts.Users;
using Pjfm.Application.Common.Mediatr;
using Pjfm.Domain.Enums;

namespace Pjfm.Application.Interfaces
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
        void AddIncludedUser(ApplicationUserDto user);
        bool TryRemoveIncludedUser(ApplicationUserDto user);
        void SetFillerQueueState(FillerQueueType fillerQueueType);
        void SetBrowserQueueSettings(BrowserQueueSettings settings);
        void DequeueTrack(string trackId);
        Response<bool> AddPriorityTrack(TrackDto track);
        Response<bool> AddSecondaryTrack(TrackDto track, ApplicationUserDto user);
        IDisposable SubscribeToPlayingStatus(IObserver<bool> observer);
    }
}