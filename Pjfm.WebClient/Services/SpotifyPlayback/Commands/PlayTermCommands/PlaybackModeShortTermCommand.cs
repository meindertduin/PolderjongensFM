using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackModeShortTermCommand : ICommand
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private TopTrackTermFilter _previousTermFilter;

        public PlaybackModeShortTermCommand(ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
        } 
        
        public void Execute()
        {
            _previousTermFilter = _spotifyPlaybackManager.CurrentTermFilter;
            _spotifyPlaybackManager.SetTermFilter(TopTrackTermFilter.ShortTerm);
        }

        public void Undo()
        {
            _spotifyPlaybackManager.SetTermFilter(_previousTermFilter);
        }
    }
}