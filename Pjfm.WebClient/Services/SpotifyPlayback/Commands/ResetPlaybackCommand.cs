using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class ResetPlaybackCommand : ICommand
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;

        public ResetPlaybackCommand(ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
        }
        public void Execute()
        {
            _spotifyPlaybackManager.ResetPlayer(0);
        }

        public void Undo()
        {
            
        }
    }
}