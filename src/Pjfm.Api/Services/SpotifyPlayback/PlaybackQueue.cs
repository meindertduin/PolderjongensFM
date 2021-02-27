using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR.Users.Queries;
using pjfm.Models;
using Pjfm.WebClient.Services.FillerQueueState;

namespace Pjfm.WebClient.Services
{
    public class PlaybackQueue : IPlaybackQueue
    {
        private readonly IServiceProvider _serviceProvider;

        private Queue<TrackDto> _fillerQueue = new Queue<TrackDto>();
        private Queue<TrackDto> _priorityQueue = new Queue<TrackDto>();
        private Queue<TrackRequestDto> _secondaryQueue = new Queue<TrackRequestDto>(); 
        
        private List<TrackDto> _recentlyPlayed = new List<TrackDto>();
        private TopTrackTermFilter _currentTermFilter;

        private IFillerQueueState _fillerQueueState;
        private PlaybackQueueSettings _playbackQueueSettings = new PlaybackQueueSettings();

        public List<ApplicationUserDto> IncludedUsers { get; private set; } = new List<ApplicationUserDto>();

        public PlaybackQueue(IServiceProvider serviceProvider, IMediator mediator)
        {
            _serviceProvider = serviceProvider;
            _fillerQueueState = new UsersTopTracksFillerQueueState(this, mediator);
            _currentTermFilter = TopTrackTermFilter.AllTerms;
        }

        public void Reset()
        {
            _recentlyPlayed = new List<TrackDto>();
            _priorityQueue = new Queue<TrackDto>();
            _secondaryQueue = new Queue<TrackRequestDto>();
            _fillerQueue = new Queue<TrackDto>();
        }

        TopTrackTermFilter IPlaybackQueue.CurrentTermFilter
        {
            get => _currentTermFilter;
            set => _currentTermFilter = value;
        }
        public PlaybackQueueSettings PlaybackQueueSettings => _playbackQueueSettings;
        
        public async Task SetUsers()
        {
            using var scope = _serviceProvider.CreateScope();
            
            // sets all users that are member to be included
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var membersResult = await mediator.Send(new GetAllPjMembersQuery());
            if (membersResult.Error == false)
            {
                IncludedUsers = membersResult.Data;
            }
        }
        
        public void AddUsersToIncludedUsers(ApplicationUserDto user)
        {
            if (IncludedUsers.Select(x => x.Id).Contains(user.Id) == false)
            {
                IncludedUsers.Add(user);
            }
        }

        public bool TryRemoveUserFromIncludedUsers(ApplicationUserDto user)
        {
            var item = IncludedUsers.SingleOrDefault(x => x.Id == user.Id);
            if (item != null)
            {
                IncludedUsers.Remove(item);
                return true;
            }

            return false;
        }
        
        public int RecentlyPlayedCount()
        {
            return _recentlyPlayed.Count;
        }
        
        public void SetTermFilter(TopTrackTermFilter termFilter)
        {
            _playbackQueueSettings.TopTrackTermFilter = termFilter;
        }
        
        public bool TryDequeueTrack(string trackId)
        {
            TrackDto[] tracks = { };
            Action<TrackDto> addTrackAction = null;
            
            // based in what queue the track is in, clear queue and move out tracks
            if (_priorityQueue.Any(x => x.Id == trackId))
            {
                tracks = _priorityQueue.ToArray();
                _priorityQueue.Clear();
                addTrackAction = (track) => _priorityQueue.Enqueue(track);
            }
            else if(_fillerQueue.Any(x => x.Id == trackId))
            {
                tracks = _fillerQueue.ToArray();
                _fillerQueue.Clear();
                addTrackAction = (track) => _fillerQueue.Enqueue(track);
            }
            else if (_secondaryQueue.Any(x => x.Track.Id == trackId))
            {
                // handle secondary queue differently as the items are different
                HandleDequeueSecondaryQueueTracks(trackId);
                return true;
            }

            if (addTrackAction != null)
            {
                // iterate over tracks and invoke add track to queue action except for track with track id
                for (int i = 0; i < tracks.Length; i++)
                {
                    if (tracks[i].Id != trackId)
                    {
                        addTrackAction.Invoke(tracks[i]);
                    }
                }

                return true;
            }

            return false;
        }

        private void HandleDequeueSecondaryQueueTracks(string trackId)
        {
            var queueItems = _secondaryQueue.ToArray();
            _secondaryQueue.Clear();

            for (int i = 0; i < queueItems.Length; i++)
            {
                if (queueItems[i].Track.Id != trackId)
                {
                    _secondaryQueue.Enqueue(queueItems[i]);
                }
            }
        }
        public void AddPriorityTrack(TrackDto track)
        {
            _priorityQueue.Enqueue(track);
        }

        public void AddSecondaryTrack(TrackRequestDto trackRequest)
        {
            _secondaryQueue.Enqueue(trackRequest);
        }

        public List<TrackDto> GetFillerQueueTracks()
        {
            return GetTracksOfQueue(_fillerQueue);
        }

        public List<TrackDto> GetPriorityQueueTracks()
        {
            return GetTracksOfQueue(_priorityQueue);
        }

        public List<TrackDto> GetSecondaryQueueTracks()
        {
            return GetTracksOfQueue(_secondaryQueue);
        }

        public List<TrackRequestDto> GetSecondaryQueueRequests()
        {
            var result = new List<TrackRequestDto>();

            foreach (var request in _secondaryQueue)
            {
                result.Add(request);
            }
            
            return result;
        }
        
        private List<TrackDto> GetTracksOfQueue(Queue<TrackDto> queue)
        {
            var result = new List<TrackDto>();

            foreach (var track in queue)
            {
                result.Add(track);
            }
            
            return result;
        }
        
        private List<TrackDto> GetTracksOfQueue(Queue<TrackRequestDto> queue)
        {
            var result = new List<TrackDto>();
            
            // maps the trackRequestDto in queue to TrackDto
            foreach (var request in queue)
            {
                var trackDto = new TrackDto();

                trackDto = request.Track;
                trackDto.User = request.User;

                result.Add(trackDto);
            }
            
            return result;
        }
        
        public async Task<TrackDto> GetNextQueuedTrack()
        {
            // gets the next queuedTrack based on the hierarchy of queues to pick out off
            TrackDto nextTrack;
            
            if (_priorityQueue.Count > 0)
            {
                nextTrack = _priorityQueue.Dequeue();
            }
            else if (_secondaryQueue.Count > 0)
            {
                nextTrack = _secondaryQueue.Dequeue().Track;
            }
            else
            {
                await AddToFillerQueue(1);
                nextTrack = _fillerQueue.Dequeue();
            }
            
            _recentlyPlayed.Add(nextTrack);
            
            return nextTrack;
        }
        public async Task AddToFillerQueue(int amount)
        {
            var result = await _fillerQueueState.RetrieveFillerTracks(amount);
            if (result.Error == false)
            {
                // adds queried tracks to the fillerQueue
                foreach (var fillerTrack in result.Data)
                {
                    _fillerQueue.Enqueue(fillerTrack);
                }
            }
        }
        
    }
}