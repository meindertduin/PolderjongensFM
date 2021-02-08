namespace Pjfm.WebClient.Services
{
    public enum PlaybackControllerCommands
    {
        TrackPlayerOnOff = 0,
        ShortTermFilterMode = 1,
        MediumTermFilterMode = 2,
        LongTermFilterMode = 3,
        ShortMediumTermFilterMode = 4,
        ResetPlaybackCommand = 5,
        AllTermFilterMode = 6,
        MediumLongTermFilterMode = 7,
        TrackSkip = 8,
        SetDefaultPlaybackState = 9,
        SetUserRequestPlaybackState = 10,
        SetRandomRequestPlaybackState = 11,
        SetRoundRobinPlaybackState = 12,
    }
}