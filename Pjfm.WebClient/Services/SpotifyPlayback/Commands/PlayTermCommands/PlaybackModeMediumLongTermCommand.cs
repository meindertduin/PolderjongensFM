using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
{
    public class PlaybackModeMediumLongTermCommand : ICommand
    {
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        private TopTrackTermFilter _previousTermFilter;

        public PlaybackModeMediumLongTermCommand(ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _spotifyPlaybackManager = spotifyPlaybackManager;
        } 
        
        public void Execute()
        {
            _previousTermFilter = _spotifyPlaybackManager.CurrentTermFilter;
            _spotifyPlaybackManager.SetTermFilter(TopTrackTermFilter.MediumLongTerm);
        }

        public void Undo()
        {
            _spotifyPlaybackManager.SetTermFilter(_previousTermFilter);
        }
    }
}