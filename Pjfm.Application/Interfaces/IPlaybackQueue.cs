﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Pjfm.Application.Common.Dto;
using Pjfm.Domain.Entities;
using pjfm.Models;

namespace Pjfm.WebClient.Services
{
    public interface IPlaybackQueue
    {
        public int RecentlyPlayedCount();
        void Reset();
        TopTrackTermFilter CurrentTermFilter { get; protected set; }
        List<ApplicationUserDto> IncludedUsers { get; }
        void SetTermFilter(TopTrackTermFilter termFilter);
        void AddUsersToIncludedUsers(ApplicationUserDto user);
        void RemoveUserFromIncludedUsers(ApplicationUserDto user);
        public void AddPriorityTrack(TrackDto track);
        void AddSecondaryTrack(TrackRequestDto trackRequest);
        public List<TrackDto> GetFillerQueueTracks();
        public List<TrackDto> GetPriorityQueueTracks();
        List<TrackDto> GetSecondaryQueueTracks();
        List<TrackRequestDto> GetSecondaryQueueRequests();
        public Task<TrackDto> GetNextQueuedTrack();
        public Task AddToFillerQueue(int amount);
    }
}