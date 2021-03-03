using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Interfaces;

namespace Pjfm.Api.Services.SpotifyPlayback.Commands
{
    public class ResetPlaybackCommand : ICommand
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;

        public ResetPlaybackCommand(ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
        }
        public async void Execute()
        {
            await _spotifyPlaybackManager.ResetPlayingTracks(0);
        }

        public void Undo()
        {
            
        }
    }
}