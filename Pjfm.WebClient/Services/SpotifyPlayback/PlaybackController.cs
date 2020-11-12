using System;
using System.Collections.Generic;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR;
using Pjfm.Domain.Interfaces;
using Pjfm.WebClient.Services.PlaybackStateCommands;

namespace Pjfm.WebClient.Services
{
    public class PlaybackController : IPlaybackController
    {
        private readonly IPlaybackQueue _playbackQueue;
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private readonly IPlaybackEventTransmitter _playbackEventTransmitter;

        private readonly ICommand[] _onCommands = new ICommand[20];
        private readonly ICommand[] _offCommands = new ICommand[20];
        private ICommand _undoCommand;
        

        public PlaybackController(IPlaybackQueue playbackQueue, ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _playbackQueue = playbackQueue;
            _spotifyPlaybackManager = spotifyPlaybackManager;
            
            IPlaybackController.CurrentPlaybackState = new UserRequestPlaybackState(_playbackQueue);
            
            _undoCommand = new NoCommand();

            _onCommands[0] = new PlaybackOnCommand(_spotifyPlaybackManager);
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
            IPlaybackController.CurrentPlaybackState = state;
        }
        
        public void TurnOn(PlaybackControllerCommands command)
        {
            _onCommands[(int) command].Execute();
            _undoCommand = _onCommands[(int) command];
        }

        public void TurnOff(PlaybackControllerCommands command)
        {
            _offCommands[(int) command].Execute();
            _undoCommand = _offCommands[(int) command];
        }

        public void Undo()
        {
            _undoCommand.Execute();
        }

        public void SetUsersInclusionList(List<ApplicationUserDto> users)
        {
            _playbackQueue.SetIncludedUsers(users);
        }

        public Response<bool> AddPriorityTrack(TrackDto track)
        {
            return IPlaybackController.CurrentPlaybackState.AddPriorityTrack(track);
        }

        public Response<bool> AddSecondaryTrack(TrackDto track, string userId)
        {
            return IPlaybackController.CurrentPlaybackState.AddSecondaryTrack(track, userId);
        }

        public List<TrackDto> GetPriorityQueueTracks()
        {
            return _playbackQueue.GetPriorityQueueTracks();
        }
        
        public List<TrackDto> GetSecondaryQueueTracks()
        {
            return _playbackQueue.GetSecondaryQueueTracks();
        }
        
        public List<TrackDto> GetFillerQueueTracks()
        {
            return _playbackQueue.GetFillerQueueTracks();
        }

        public Tuple<TrackDto, DateTime> GetPlayingTrackInfo()
        {
            var result = new Tuple<TrackDto, DateTime>(
                _spotifyPlaybackManager.CurrentPlayingTrack,
                _spotifyPlaybackManager.CurrentTrackStartTime);

            return result;
        }

        public IDisposable SubscribeToPlayingStatus(IObserver<bool> observer)
        {
            return _spotifyPlaybackManager.Subscribe(observer);
        }
    }
}