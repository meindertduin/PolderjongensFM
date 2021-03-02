using Pjfm.Application.Interfaces;
using pjfm.Interfaces;
using pjfm.Models;

namespace Pjfm.WebClient.Services
{
    public class PlaybackInfoFactory : IPlaybackInfoFactory
    {
        private readonly IPlaybackController _playbackController;
        private readonly IPlaybackInfoProvider _playbackInfoProvider;

        public PlaybackInfoFactory(IPlaybackController playbackController, IPlaybackInfoProvider playbackInfoProvider)
        {
            _playbackController = playbackController;
            _playbackInfoProvider = playbackInfoProvider;
        }
        
        public UserPlaybackInfoModel CreateUserInfoModel()
        {
            var infoModel = new UserPlaybackInfoModel();
            FillInBaseValues(infoModel);
            
            var fillerQueuedTracks = _playbackInfoProvider.GetFillerQueueTracks();
            var secondaryQueueTracks = _playbackInfoProvider.GetSecondaryQueueTracks();
            var queuedPriorityTracks = _playbackInfoProvider.GetPriorityQueueTracks();
            
            infoModel.PriorityQueuedTracks = queuedPriorityTracks;
            infoModel.SecondaryQueuedTracks = secondaryQueueTracks;
            infoModel.FillerQueuedTracks = fillerQueuedTracks;

            return infoModel;
        }

        public DjPlaybackInfoModel CreateDjInfoModel()
        {
            var infoModel = new DjPlaybackInfoModel();
            FillInBaseValues(infoModel);
            
            var fillerQueuedTracks = _playbackInfoProvider.GetFillerQueueTracks();
            var secondaryQueueTracks = _playbackInfoProvider.GetSecondaryQueueTracks();
            var queuedPriorityTracks = _playbackInfoProvider.GetPriorityQueueTracks();

            infoModel.PriorityQueuedTracks = queuedPriorityTracks;
            infoModel.SecondaryQueuedTracks = secondaryQueueTracks;
            infoModel.FillerQueuedTracks = fillerQueuedTracks;
            
            return infoModel;
        }

        private void  FillInBaseValues(PlayerUpdateInfoModel infoModel)
        {
            var playingTrackInfo = _playbackInfoProvider.GetPlayingTrackInfo();
            
            infoModel.CurrentPlayingTrack = playingTrackInfo.Item1;
            infoModel.StartingTime = playingTrackInfo.Item2;
        }
    }
}