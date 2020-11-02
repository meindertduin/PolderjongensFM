using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlayModeTermOffCommand : ICommand
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private TopTrackTermFilter _previousTermFilter;

        public PlayModeTermOffCommand(ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
        } 
        
        public void Execute()
        {
            _previousTermFilter = _spotifyPlaybackManager.CurrentTermFilter;
            _spotifyPlaybackManager.SetTermFilter(TopTrackTermFilter.AllTerms);
        }

        public void Undo()
        {
            _spotifyPlaybackManager.SetTermFilter(_previousTermFilter);
        }
    }
}