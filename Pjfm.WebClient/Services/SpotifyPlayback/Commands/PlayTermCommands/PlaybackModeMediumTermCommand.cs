using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackModeMediumTermCommand : ICommand
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private TopTrackTermFilter _previousTermFilter;

        public PlaybackModeMediumTermCommand(ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
        } 
        
        public void Execute()
        {
            _previousTermFilter = _spotifyPlaybackManager.CurrentTermFilter;
            _spotifyPlaybackManager.SetTermFilter(TopTrackTermFilter.MediumTerm);
        }

        public void Undo()
        {
            _spotifyPlaybackManager.SetTermFilter(_previousTermFilter);
        }
    }
}