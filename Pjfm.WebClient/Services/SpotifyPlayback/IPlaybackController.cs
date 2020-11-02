namespace Pjfm.WebClient.Services
{
    public interface IPlaybackController
    {
        void TurnOn(PlaybackControllerCommands command);
        void TurnOff(PlaybackControllerCommands command);
        void Undo();
    }
}