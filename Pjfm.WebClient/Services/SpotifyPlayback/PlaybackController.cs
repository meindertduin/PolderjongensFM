using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.Domain.Enums;
using Pjfm.Domain.Interfaces;
using pjfm.Hubs;
using pjfm.Models;
using Pjfm.WebClient.Services.PlaybackStateCommands;

namespace Pjfm.WebClient.Services
{
    public class PlaybackController : IPlaybackController
    {
        private readonly IPlaybackQueue _playbackQueue;
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private readonly IHubContext<DjHub> _djHubContext;
        private readonly IHubContext<RadioHub> _radioHubContext;

        private readonly ICommand[] _onCommands = new ICommand[20];
        private readonly ICommand[] _offCommands = new ICommand[20];
        private ICommand _undoCommand;
        

        public PlaybackController(IPlaybackQueue playbackQueue, ISpotifyPlaybackManager spotifyPlaybackManager,
            IHubContext<DjHub> djHubContext, IHubContext<RadioHub> radioHubContext)
        {
            _playbackQueue = playbackQueue;
            _spotifyPlaybackManager = spotifyPlaybackManager;
            _djHubContext = djHubContext;
            _radioHubContext = radioHubContext;
            
            SetPlaybackState(new UserRequestPlaybackState(_playbackQueue)); // Default 

            _undoCommand = new NoCommand();

            _onCommands[0] = new PlaybackOnCommand(this ,_spotifyPlaybackManager, _playbackQueue);
            _offCommands[0] = new PlaybackOffCommand(_spotifyPlaybackManager);

            _onCommands[1] = new PlaybackModeShortTermCommand(_playbackQueue);
            _offCommands[1] = new NoCommand();
            
            _onCommands[2] = new PlaybackModeMediumTermCommand(_playbackQueue);
            _offCommands[2] = new NoCommand();
            
            _onCommands[3] = new PlaybackModeLongTermCommand(_playbackQueue);
            _offCommands[3] = new NoCommand();
            
            _onCommands[4] = new PlaybackModeShortMediumTermCommand(_playbackQueue);
            _offCommands[4] = new NoCommand();

            _onCommands[5] = new ResetPlaybackCommand(_spotifyPlaybackManager);
            _offCommands[5] = new NoCommand();

            _onCommands[6] = new PlaybackModeAllTermCommand(_playbackQueue);
            _offCommands[6] = new NoCommand();
            
            _onCommands[7] = new PlaybackModeMediumLongTermCommand(_playbackQueue);
            _offCommands[7] = new NoCommand();
            
            _onCommands[8] = new PlaybackSkipCommand(_spotifyPlaybackManager);
            _offCommands[8] = new NoCommand();
            
            _onCommands[9] = new DefaultPlaybackStateOnCommand(this, _playbackQueue);
            _offCommands[9] = new NoCommand();
            
            _onCommands[10] = new UserRequestPlaybackStateOnCommand(this, _playbackQueue);
            _offCommands[10] = new NoCommand();
            
            _onCommands[11] = new RandomRequestPlaybackStateOnCommand(this, _playbackQueue);
            _offCommands[11] = new NoCommand();
            
        }
        
        public void SetPlaybackState(IPlaybackState state)
        {
            if (IPlaybackController.CurrentPlaybackState != null)
            {
                var maxRequestsAmount = IPlaybackController.CurrentPlaybackState.GetMaxRequestsPerUser();
                state.SetMaxRequestsPerUser(maxRequestsAmount);
            }
            
            IPlaybackController.CurrentPlaybackState = state;
            
            NotifyChangePlaybackSettings();
        }

        public void SetMaxRequestsPerUserAmount(int amount)
        {
            IPlaybackController.CurrentPlaybackState.SetMaxRequestsPerUser(amount);
        }
        
        public void TurnOn(PlaybackControllerCommands command)
        {
            _onCommands[(int) command].Execute();
            _undoCommand = _onCommands[(int) command];
            NotifyChangePlaybackSettings();
        }

        public void TurnOff(PlaybackControllerCommands command)
        {
            _offCommands[(int) command].Execute();
            _undoCommand = _offCommands[(int) command];
            NotifyChangePlaybackSettings();
        }

        public void Undo()
        {
            _undoCommand.Execute();
            NotifyChangePlaybackSettings();
        }

        public Task SynchWithPlayback(string userId, string spotifyAccessToken)
        {
            return _spotifyPlaybackManager.SynchWithCurrentPlayer(userId, spotifyAccessToken);
        }

        public List<ApplicationUserDto> GetIncludedUsers()
        {
            return _playbackQueue.IncludedUsers;
        }

        public void AddIncludedUser(ApplicationUserDto user)
        {
            _playbackQueue.AddUsersToIncludedUsers(user);
        }

        public bool TryRemoveIncludedUser(ApplicationUserDto user)
        {
            return _playbackQueue.TryRemoveUserFromIncludedUsers(user);
        }

        public Response<bool> AddPriorityTrack(TrackDto track)
        {
            return IPlaybackController.CurrentPlaybackState.AddPriorityTrack(track);
        }
        public Response<bool> AddSecondaryTrack(TrackDto track, ApplicationUserDto user)
        {
            return IPlaybackController.CurrentPlaybackState.AddSecondaryTrack(track, user);
        }
        
        public List<TrackDto> GetPriorityQueueTracks()
        {
            return _playbackQueue.GetPriorityQueueTracks();
        }
        public List<TrackDto> GetSecondaryQueueTracks()
        {
            return IPlaybackController.CurrentPlaybackState.GetSecondaryTracks();
        }
        public List<TrackDto> GetFillerQueueTracks()
        {
            return _playbackQueue.GetFillerQueueTracks();
        }
        public PlaybackSettingsDto GetPlaybackSettings()
        {
            PlaybackState currentPlaybackState = PlaybackState.RequestPlaybackState;

            if (IPlaybackController.CurrentPlaybackState is DefaultPlaybackState)
                currentPlaybackState = PlaybackState.DefaultPlaybackState;
            if (IPlaybackController.CurrentPlaybackState is UserRequestPlaybackState)
                currentPlaybackState = PlaybackState.RequestPlaybackState;
            if (IPlaybackController.CurrentPlaybackState is RandomRequestPlaybackState)
                currentPlaybackState = PlaybackState.RandomRequestPlaybackState;

            var maxRequestsPerUser = IPlaybackController.CurrentPlaybackState != null
                ? IPlaybackController.CurrentPlaybackState.GetMaxRequestsPerUser()
                : 0;

            var playbackSettings = new PlaybackSettingsDto()
            {
                IsPlaying = _spotifyPlaybackManager.IsCurrentlyPlaying,
                PlaybackTermFilter = _playbackQueue.CurrentTermFilter,
                PlaybackState = currentPlaybackState,
                IncludedUsers = _playbackQueue.IncludedUsers,
                MaxRequestsPerUser = maxRequestsPerUser,
            };

            return playbackSettings;
        }
        
        public Tuple<TrackDto, DateTime> GetPlayingTrackInfo()
        {
            var result = new Tuple<TrackDto, DateTime>(
                _spotifyPlaybackManager.CurrentPlayingTrack,
                _spotifyPlaybackManager.CurrentTrackStartTime);

            return result;
        }

        private void NotifyChangePlaybackSettings()
        {
            var playbackSettings = GetPlaybackSettings();
            _djHubContext.Clients.All.SendAsync("PlaybackSettings", playbackSettings);
            _radioHubContext.Clients.All.SendAsync("PlaybackSettings", new UserPlaybackSettingsModel()
            {
                PlaybackState = playbackSettings.PlaybackState,
                IsPlaying = playbackSettings.IsPlaying,
                MaxRequestsPerUser = playbackSettings.MaxRequestsPerUser,
            });
        }

        public IDisposable SubscribeToPlayingStatus(IObserver<bool> observer)
        {
            return _spotifyPlaybackManager.Subscribe(observer);
        }
    }
}