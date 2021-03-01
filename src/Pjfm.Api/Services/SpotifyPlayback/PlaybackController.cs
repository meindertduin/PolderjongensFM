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
using Pjfm.WebClient.Services.FillerQueueState;
using Pjfm.WebClient.Services.PlaybackStateCommands;

namespace Pjfm.WebClient.Services
{
    public class PlaybackController : IPlaybackController
    {
        private readonly IPlaybackQueue _playbackQueue;
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private readonly IHubContext<DjHub> _djHubContext;
        private readonly IHubContext<RadioHub> _radioHubContext;

        private ICommand _undoCommand;

        public PlaybackController(IPlaybackQueue playbackQueue, ISpotifyPlaybackManager spotifyPlaybackManager,
            IHubContext<DjHub> djHubContext, IHubContext<RadioHub> radioHubContext)
        {
            _playbackQueue = playbackQueue;
            _spotifyPlaybackManager = spotifyPlaybackManager;
            _djHubContext = djHubContext;
            _radioHubContext = radioHubContext;

            _undoCommand = new NoCommand();
        }

        public void SetPlaybackState(IPlaybackState state)
        {
            if (IPlaybackController.CurrentPlaybackState != null)
            {
                var maxRequestsAmount = IPlaybackController.CurrentPlaybackState.GetMaxRequestsPerUser();
                state.SetMaxRequestsPerUser(maxRequestsAmount);
            }

            IPlaybackController.CurrentPlaybackState = state;
        }

        public void SetMaxRequestsPerUserAmount(int amount)
        {
            if (IPlaybackController.CurrentPlaybackState != null)
            {
                IPlaybackController.CurrentPlaybackState.SetMaxRequestsPerUser(amount);
            }
        }

        public void TurnOn(PlaybackControllerCommands command)
        {
            var commandHandler = GetOnCommandHandler(command);
            commandHandler.Execute();
            _undoCommand = commandHandler;
            NotifyChangePlaybackSettings();
        }

        public void TurnOff(PlaybackControllerCommands command)
        {
            var commandHandler = GetOffCommandHandler(command);
            commandHandler.Execute();
            _undoCommand = commandHandler;
            NotifyChangePlaybackSettings();
        }

        private ICommand GetOnCommandHandler(PlaybackControllerCommands command) =>
            command switch
            {
                PlaybackControllerCommands.TrackPlayerOnOff => new PlaybackOnCommand(this, _spotifyPlaybackManager,
                    _playbackQueue),
                PlaybackControllerCommands.ShortTermFilterMode => new PlaybackModeShortTermCommand(_playbackQueue),
                PlaybackControllerCommands.MediumTermFilterMode => new PlaybackModeMediumTermCommand(_playbackQueue),
                PlaybackControllerCommands.LongTermFilterMode => new PlaybackModeLongTermCommand(_playbackQueue),
                PlaybackControllerCommands.ShortMediumTermFilterMode => new PlaybackModeShortMediumTermCommand(
                    _playbackQueue),
                PlaybackControllerCommands.ResetPlaybackCommand => new ResetPlaybackCommand(_spotifyPlaybackManager),
                PlaybackControllerCommands.AllTermFilterMode => new PlaybackModeAllTermCommand(_playbackQueue),
                PlaybackControllerCommands.MediumLongTermFilterMode => new PlaybackModeMediumLongTermCommand(
                    _playbackQueue),
                PlaybackControllerCommands.TrackSkip => new PlaybackSkipCommand(_spotifyPlaybackManager),
                PlaybackControllerCommands.SetDefaultPlaybackState => new DefaultPlaybackStateOnCommand(this,
                    _playbackQueue),
                PlaybackControllerCommands.SetUserRequestPlaybackState => new UserRequestPlaybackStateOnCommand(this,
                    _playbackQueue),
                PlaybackControllerCommands.SetRandomRequestPlaybackState => new RandomRequestPlaybackStateOnCommand(
                    this, _playbackQueue),
                PlaybackControllerCommands.SetRoundRobinPlaybackState => new RoundRobinPlaybackStateOnCommand(
                    this, _playbackQueue),
                _ => new NoCommand(),
            };
        
        private ICommand GetOffCommandHandler(PlaybackControllerCommands command) =>
            command switch
            {
                PlaybackControllerCommands.TrackPlayerOnOff => new PlaybackOffCommand(_spotifyPlaybackManager),
                _ => new NoCommand(),
            };

        
        public void Undo()
        {
            _undoCommand.Execute();
            NotifyChangePlaybackSettings();
        }

        public Task SynchWithPlayback(string userId, string spotifyAccessToken, PlaybackDevice playbackDevice)
        {
            return _spotifyPlaybackManager.SynchWithCurrentPlayer(userId, spotifyAccessToken, playbackDevice);
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

        public void SetFillerQueueState(FillerQueueType fillerQueueType)
        {
            _playbackQueue.SetFillerQueueState(fillerQueueType);
            NotifyChangePlaybackSettings();
        }

        public void SetBrowserQueueSettings(BrowserQueueSettings settings)
        {
            _playbackQueue.SetBrowserQueueSettings(settings);
        }

        public void DequeueTrack(string trackId)
        {
            _playbackQueue.TryDequeueTrack(trackId);
            NotifyChangePlaybackInfo();
        }

        private void NotifyChangePlaybackInfo()
        {
            var infoModelFactory = new PlaybackInfoFactory(this);
            var userInfo = infoModelFactory.CreateUserInfoModel();
            var djInfo = infoModelFactory.CreateUserInfoModel();
            
            _radioHubContext.Clients.All.SendAsync("ReceivePlaybackInfo", userInfo);
            _djHubContext.Clients.All.SendAsync("ReceiveDjPlaybackInfo", djInfo);
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
            // get secondary tracks of playbackState if not null
            if (IPlaybackController.CurrentPlaybackState != null)
            {
                var secondaryTracks = IPlaybackController.CurrentPlaybackState.GetSecondaryTracks();
                if (IPlaybackController.CurrentPlaybackState is RandomRequestPlaybackState)
                {
                     secondaryTracks.AddRange(_playbackQueue.GetSecondaryQueueTracks());
                }
                
                return secondaryTracks;
            }
            
            return new List<TrackDto>();
        }
        public List<TrackDto> GetFillerQueueTracks()
        {
            return _playbackQueue.GetFillerQueueTracks();
        }
        public PlaybackSettingsDto GetPlaybackSettings()
        {
            PlaybackState currentPlaybackState = IPlaybackController.CurrentPlaybackState switch
            {
                // get the current playbackState
                DefaultPlaybackState _ => PlaybackState.DefaultPlaybackState,
                UserRequestPlaybackState _ => PlaybackState.RequestPlaybackState,
                RandomRequestPlaybackState _ => PlaybackState.RandomRequestPlaybackState,
                RoundRobinPlaybackState _ => PlaybackState.RoundRobinPlaybackState,
                _ => PlaybackState.RequestPlaybackState
            };

            // get the maxRequestPerUser amount
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
                BrowserQueueSettings = _playbackQueue.GetBrowserQueueSettings(),
                FillerQueueState = _playbackQueue.GetFillerQueueState(),
            };

            return playbackSettings;
        }

        public bool IsPlaying()
        {
            return _spotifyPlaybackManager.IsCurrentlyPlaying;
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