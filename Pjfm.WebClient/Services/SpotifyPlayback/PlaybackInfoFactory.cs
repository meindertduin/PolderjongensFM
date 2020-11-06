﻿using pjfm.Interfaces;
using pjfm.Models;

namespace Pjfm.WebClient.Services
{
    public class PlaybackInfoFactory : IPlaybackInfoFactory
    {
        private readonly IPlaybackController _playbackController;

        public PlaybackInfoFactory(IPlaybackController playbackController)
        {
            _playbackController = playbackController;
        }
        
        public UserPlaybackInfoModel CreateUserInfoModel()
        {
            var infoModel = new UserPlaybackInfoModel();
            FillInBaseValues(infoModel);
            
            var fillerQueuedTracks = _playbackController.GetFillerQueueTracks();
            var queuedPriorityTracks = _playbackController.GetPriorityQueueTracks();

            infoModel.FillerQueuedTracks = fillerQueuedTracks;
            infoModel.PriorityQueuedTracks = queuedPriorityTracks;

            return infoModel;
        }

        public DjPlaybackInfoModel CreateDjInfoModel()
        {
            var infoModel = new DjPlaybackInfoModel();
            FillInBaseValues(infoModel);
            
            var fillerQueuedTracks = _playbackController.GetFillerQueueTracks();
            var queuedPriorityTracks = _playbackController.GetPriorityQueueTracks();

            infoModel.FillerQueuedTracks = fillerQueuedTracks;
            infoModel.PriorityQueuedTracks = queuedPriorityTracks;

            return infoModel;
        }

        private void  FillInBaseValues(PlayerUpdateInfoModel infoModel)
        {
            var playingTrackInfo = _playbackController.GetPlayingTrackInfo();

            infoModel.CurrentPlayingTrack = playingTrackInfo.Item1;
            infoModel.StartingTime = playingTrackInfo.Item2;
            
        }
    }
}