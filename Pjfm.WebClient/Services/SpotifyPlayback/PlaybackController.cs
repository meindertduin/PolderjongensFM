using System.Collections.Generic;
using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackController : IPlaybackController
    {
        private readonly IPlaybackQueue _playbackQueue;
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;

        private readonly ICommand[] _onCommands = new ICommand[10];
        private readonly ICommand[] _offCommands = new ICommand[10];
        private ICommand _undoCommand;

        public PlaybackController(IPlaybackQueue playbackQueue, ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _playbackQueue = playbackQueue;
            _spotifyPlaybackManager = spotifyPlaybackManager;
            
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
        
    }
}