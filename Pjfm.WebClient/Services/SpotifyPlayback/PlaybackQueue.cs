using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.MediatR.Users.Queries;
using Pjfm.Application.Spotify.Queries;
using pjfm.Models;
using Serilog;

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


        public List<ApplicationUserDto> IncludedUsers { get; private set; } = new List<ApplicationUserDto>();

        public PlaybackQueue(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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

        public async Task SetIncludedUsers()
        {
            using var scope = _serviceProvider.CreateScope();
            
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(new GetAllPjMembersQuery());

            if (result.Error == false)
            {
                IncludedUsers = result.Data;
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
            _currentTermFilter = termFilter;
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
            using var scope = _serviceProvider.CreateScope();
            
            var _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            
            var result = await _mediator.Send(new GetRandomTopTrackQuery()
            {
                NotIncludeTracks = _recentlyPlayed,
                RequestedAmount = amount,
                TopTrackTermFilter = _currentTermFilter.ConvertToTopTrackTerms(),
                IncludedUsersId = IncludedUsers.Select(x => x.Id).ToArray()
            });
                
            foreach (var fillerTrack in result.Data)
            {
                _fillerQueue.Enqueue(fillerTrack);
            }
        }
        
    }
}