using System;
using System.Collections.Generic;
using Pjfm.Domain.Entities;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackController
    {
        void TurnOn(PlaybackControllerCommands command);
        void TurnOff(PlaybackControllerCommands command);
        void Undo();
        void AddPriorityTrack(TopTrack track);
        List<TopTrack> GetPriorityQueueTracks();
        List<TopTrack> GetFillerQueueTracks();
        Tuple<TopTrack, DateTime> GetPlayingTrackInfo();
    }
}