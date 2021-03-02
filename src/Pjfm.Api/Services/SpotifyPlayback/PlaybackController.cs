using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Interfaces;
using Pjfm.Application.MediatR;
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
        private readonly IPlaybackInfoProvider _playbackInfoProvider;

        private ICommand _undoCommand;

        public PlaybackController(IPlaybackQueue playbackQueue, ISpotifyPlaybackManager spotifyPlaybackManager,
            IHubContext<DjHub> djHubContext, IHubContext<RadioHub> radioHubContext, IPlaybackInfoProvider playbackInfoProvider)
        {
            _playbackQueue = playbackQueue;
            _spotifyPlaybackManager = spotifyPlaybackManager;
            _djHubContext = djHubContext;
            _radioHubContext = radioHubContext;
            _playbackInfoProvider = playbackInfoProvider;

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
            var infoModelFactory = new PlaybackInfoFactory(this, _playbackInfoProvider);
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
        
        private void NotifyChangePlaybackSettings()
        {
            var playbackSettings = _playbackInfoProvider.GetPlaybackSettings();
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