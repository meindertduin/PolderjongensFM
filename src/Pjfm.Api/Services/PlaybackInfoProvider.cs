using System;
using System.Collections.Generic;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Interfaces;
using Pjfm.Domain.Enums;
using Pjfm.Domain.Interfaces;
using Pjfm.WebClient.Services;

namespace Pjfm.Api.Services
{
    public class PlaybackInfoProvider : IPlaybackInfoProvider
    {
        private readonly IPlaybackQueue _playbackQueue;
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;

        public PlaybackInfoProvider(IPlaybackQueue playbackQueue, ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _playbackQueue = playbackQueue;
            _spotifyPlaybackManager = spotifyPlaybackManager;
        }
        
        public List<ApplicationUserDto> GetIncludedUsers()
        {
            return _playbackQueue.IncludedUsers;
        }
        
        public List<TrackDto> GetPriorityQueueTracks()
        {
            return _playbackQueue.GetPriorityQueueTracks();
        }
        
        public List<TrackDto> GetSecondaryQueueTracks()
        {
            // get secondary tracks of playbackState if not null
            if (IPlaybackController.CurrentPlaybackState != null)
            {
                var secondaryTracks = IPlaybackController.CurrentPlaybackState.GetSecondaryTracks();
                if (IPlaybackController.CurrentPlaybackState is RandomRequestPlaybackState)
                {
                     secondaryTracks.AddRange(_playbackQueue.GetSecondaryQueueTracks());
                }
                
                return secondaryTracks;
            }
            
            return new List<TrackDto>();
        }
        
        public List<TrackDto> GetFillerQueueTracks()
        {
            return _playbackQueue.GetFillerQueueTracks();
        }

        public Tuple<TrackDto, DateTime> GetPlayingTrackInfo()
        {
            var result = new Tuple<TrackDto, DateTime>(
                _spotifyPlaybackManager.CurrentPlayingTrack,
                _spotifyPlaybackManager.CurrentTrackStartTime);

            return result;
        }

        public PlaybackSettingsDto GetPlaybackSettings()
        {
            PlaybackState currentPlaybackState = IPlaybackController.CurrentPlaybackState switch
            {
                // get the current playbackState
                DefaultPlaybackState _ => PlaybackState.DefaultPlaybackState,
                UserRequestPlaybackState _ => PlaybackState.RequestPlaybackState,
                RandomRequestPlaybackState _ => PlaybackState.RandomRequestPlaybackState,
                RoundRobinPlaybackState _ => PlaybackState.RoundRobinPlaybackState,
                _ => PlaybackState.RequestPlaybackState
            };

            // get the maxRequestPerUser amount
            var maxRequestsPerUser = IPlaybackController.CurrentPlaybackState != null
                ? IPlaybackController.CurrentPlaybackState.GetMaxRequestsPerUser()
                : 0;

            var playbackSettings = new PlaybackSettingsDto()
            {
                IsPlaying = _spotifyPlaybackManager.IsCurrentlyPlaying,
                PlaybackTermFilter = _playbackQueue.CurrentTermFilter,
                PlaybackState = currentPlaybackState,
                IncludedUsers = _playbackQueue.IncludedUsers,
                MaxRequestsPerUser = maxRequestsPerUser,
                BrowserQueueSettings = _playbackQueue.GetBrowserQueueSettings(),
                FillerQueueState = _playbackQueue.GetFillerQueueState(),
            };

            return playbackSettings;
        }

        public bool IsPlaying()
        {
            return _spotifyPlaybackManager.IsCurrentlyPlaying;
        }
    }
}