using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackModeShortMediumTermCommand : ICommand
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private TopTrackTermFilter _previousTermFilter;

        public PlaybackModeShortMediumTermCommand(ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
        } 
        
        public void Execute()
        {
            _previousTermFilter = _spotifyPlaybackManager.CurrentTermFilter;
            _spotifyPlaybackManager.SetTermFilter(TopTrackTermFilter.ShortMediumTerm);
        }

        public void Undo()
        {
            _spotifyPlaybackManager.SetTermFilter(_previousTermFilter);
        }
    }
}