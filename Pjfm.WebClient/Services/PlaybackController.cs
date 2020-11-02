using System.Collections.Generic;
using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackController : IPlaybackController
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        
        private readonly List<ICommand> _onCommands = new List<ICommand>();
        private readonly List<ICommand> _offCommands = new List<ICommand>();
        private ICommand _undoCommand;

        public PlaybackController(ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
            _undoCommand = new NoCommand();

            _onCommands[0] = new PlaybackOnCommand(_spotifyPlaybackManager);
            _offCommands[0] = new PlaybackOffCommand(_spotifyPlaybackManager);

            _onCommands[1] = new PlaybackModeShortTermCommand(_spotifyPlaybackManager);
            _offCommands[1] = new NoCommand();
            
            _onCommands[2] = new PlaybackModeMediumTermCommand(_spotifyPlaybackManager);
            _offCommands[2] = new NoCommand();
            
            _onCommands[3] = new PlaybackModeLongTermCommand(_spotifyPlaybackManager);
            _offCommands[3] = new NoCommand();
            
            _onCommands[4] = new PlaybackModeShortMediumTermCommand(_spotifyPlaybackManager);
            _offCommands[4] = new NoCommand();
            
            _onCommands[5] = new PlaybackModeMediumLongTermCommand(_spotifyPlaybackManager);
            _offCommands[5] = new NoCommand();
            
            _onCommands[5] = new ResetPlaybackCommand(_spotifyPlaybackManager);
            _offCommands[5] = new NoCommand();

            _onCommands[6] = new PlaybackModeAllTermCommand(_spotifyPlaybackManager);
            _offCommands[6] = new NoCommand();
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