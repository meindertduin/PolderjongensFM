using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Interfaces;

namespace Pjfm.Api.Services.SpotifyPlayback.Commands
{
    public class PlaybackSkipCommand : ICommand
    {
        private readonly ISpotifyPlaybackManager _playbackManager;

        public PlaybackSkipCommand(ISpotifyPlaybackManager playbackManager)
        {
            _playbackManager = playbackManager;
        }
        public void Execute()
        {
            _playbackManager.SkipTrack();
        }

        public void Undo()
        {
            
        }
    }
}