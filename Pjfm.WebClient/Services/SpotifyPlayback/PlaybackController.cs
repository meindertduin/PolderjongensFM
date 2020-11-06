using System;
using System.Collections.Generic;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Entities;
using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackController : IPlaybackController
    {
        private readonly IPlaybackQueue _playbackQueue;
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private readonly IPlaybackEventTransmitter _playbackEventTransmitter;

        private readonly ICommand[] _onCommands = new ICommand[10];
        private readonly ICommand[] _offCommands = new ICommand[10];
        private ICommand _undoCommand;
        

        public PlaybackController(IPlaybackQueue playbackQueue, ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _playbackQueue = playbackQueue;
            _spotifyPlaybackManager = spotifyPlaybackManager;
            
            IPlaybackController.CurrentPlaybackState = new DefaultPlaybackState(_playbackQueue);
            
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

        public void AddPriorityTrack(TrackDto track)
        {
            _playbackQueue.AddPriorityTrack(track);
        }

        public void AddSecondaryTrack(TrackDto track)
        {
            _playbackQueue.AddSecondaryTrack(track);
        }

        public List<TrackDto> GetPriorityQueueTracks()
        {
            return _playbackQueue.GetPriorityQueueTracks();
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