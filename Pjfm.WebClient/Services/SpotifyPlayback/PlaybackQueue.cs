using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Spotify.Queries;

namespace Pjfm.WebClient.Services
{
    public class PlaybackQueue : IPlaybackQueue
    {
        private readonly IServiceProvider _serviceProvider;
        
        private Queue<TrackDto> _fillerQueue = new Queue<TrackDto>();
        private Queue<TrackDto> _priorityQueue = new Queue<TrackDto>();
        private Queue<TrackDto> _secondaryQueue = new Queue<TrackDto>(); 
        
        private List<TrackDto> _recentlyPlayed = new List<TrackDto>();

        public TopTrackTermFilter CurrentTermFilter { get; private set; } = TopTrackTermFilter.AllTerms;
        
        public List<ApplicationUserDto> IncludedUsers { get; private set; } = new List<ApplicationUserDto>();

        public PlaybackQueue(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Reset()
        {
            _recentlyPlayed = new List<TrackDto>();
            _priorityQueue = new Queue<TrackDto>();
            _fillerQueue = new Queue<TrackDto>();
            IncludedUsers = new List<ApplicationUserDto>();
        }

        public void SetIncludedUsers(List<ApplicationUserDto> users)
        {
            IncludedUsers = users;
        }
        
        public int RecentlyPlayedCount()
        {
            return _recentlyPlayed.Count;
        }
        
        public void SetTermFilter(TopTrackTermFilter termFilter)
        {
            CurrentTermFilter = termFilter;
        }
        
        public void AddPriorityTrack(TrackDto track)
        {
            _priorityQueue.Enqueue(track);
        }

        public void AddSecondaryTrack(TrackDto track)
        {
            _secondaryQueue.Enqueue(track);
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
        
        public List<TrackDto> GetTracksOfQueue(Queue<TrackDto> queue)
        {
            var result = new List<TrackDto>();

            foreach (var track in queue)
            {
                result.Add(track);
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
                nextTrack = _secondaryQueue.Dequeue();
            }
            else
            {
                await AddToFillerQueue(1);
                nextTrack = _fillerQueue.Dequeue();
            }
            
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
                TopTrackTermFilter = CurrentTermFilter.ConvertToTopTrackTerms(),
                IncludedUsersId = IncludedUsers.Select(x => x.Id).ToArray()
            });

            foreach (var fillerTrack in result.Data)
            {
                _fillerQueue.Enqueue(fillerTrack);
            }
        }
        
    }
}