using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pjfm.Application.Spotify.Queries;
using Pjfm.Domain.Entities;

namespace Pjfm.WebClient.Services
{
    public class PlaybackQueue : IPlaybackQueue
    {
        private readonly IServiceProvider _serviceProvider;
        private Queue<TopTrack> _fillerQueue = new Queue<TopTrack>();
        private Queue<TopTrack> _priorityQueue = new Queue<TopTrack>();
        
        private List<TopTrack> _recentlyPlayed = new List<TopTrack>();

        public TopTrackTermFilter CurrentTermFilter { get; private set; } = TopTrackTermFilter.AllTerms;
        
        public PlaybackQueue(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Reset()
        {
            _recentlyPlayed = new List<TopTrack>();
            _priorityQueue = new Queue<TopTrack>();
            _fillerQueue = new Queue<TopTrack>();
        }
        
        public int RecentlyPlayedCount()
        {
            return _recentlyPlayed.Count;
        }
        
        public void SetTermFilter(TopTrackTermFilter termFilter)
        {
            CurrentTermFilter = termFilter;
        }
        
        public void AddPriorityTrack(TopTrack track)
        {
            _priorityQueue.Enqueue(track);
        }

        public List<TopTrack> GetFillerQueueTracks()
        {
            return GetTracksOfQueue(_fillerQueue);
        }

        public List<TopTrack> GetPriorityQueueTracks()
        {
            return GetTracksOfQueue(_priorityQueue);
        }
        
        public List<TopTrack> GetTracksOfQueue(Queue<TopTrack> queue)
        {
            var result = new List<TopTrack>();

            foreach (var track in queue)
            {
                result.Add(track);
            }
            
            return result;
        }
        
        public async Task<TopTrack> GetNextQueuedTrack()
        {
            TopTrack nextTrack;
            
            if (_priorityQueue.Count > 0)
            {
                nextTrack = _priorityQueue.Dequeue();
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
                TopTrackTermFilter = CurrentTermFilter,
            });

            foreach (var fillerTrack in result.Data)
            {
                _fillerQueue.Enqueue(fillerTrack);
            }
        }
    }

    public interface IPlaybackQueue
    {
        public int RecentlyPlayedCount();
        void Reset();
        TopTrackTermFilter CurrentTermFilter { get; }
        void SetTermFilter(TopTrackTermFilter termFilter);
        public void AddPriorityTrack(TopTrack track);
        public List<TopTrack> GetFillerQueueTracks();
        public List<TopTrack> GetPriorityQueueTracks();
        public Task<TopTrack> GetNextQueuedTrack();
        public Task AddToFillerQueue(int amount);
    }
}