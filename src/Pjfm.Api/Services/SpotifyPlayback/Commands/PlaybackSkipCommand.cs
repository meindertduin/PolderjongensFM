using Pjfm.Domain.Interfaces;

namespace Pjfm.WebClient.Services
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