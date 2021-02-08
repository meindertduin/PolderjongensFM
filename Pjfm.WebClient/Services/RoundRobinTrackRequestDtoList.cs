using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using pjfm.Models;

namespace Pjfm.WebClient.Services
{
    public class RoundRobinTrackRequestDtoList<T> : ICollection
    {
        private Queue<Queue<TrackRequestDto>> _innerObjects = new Queue<Queue<TrackRequestDto>>();
        private int _count = 0;
        
        public IEnumerator GetEnumerator()
        {
            var values = GetValues().ToArray();
            return values.GetEnumerator();
        }
        
        public IEnumerable<TrackRequestDto> GetValues()
        {
            var groups = _innerObjects.ToArray();
            var indexedRequests = new List<(int Index, TrackRequestDto Request)>();

            for (int i = 0; i < groups.Length; i++)
            {
                foreach (var request in groups[i])
                {
                    indexedRequests.Add((i, request));
                }
            }

            return indexedRequests
                .GroupBy(pair => pair.Index)
                .SelectMany(group => group.Select((request, index) => new
                    {
                        Value = request,
                        GroupIndex = request.Index,
                        Index = index,
                    }))
                .OrderBy(v => v.Index)
                .ThenBy(v => v.GroupIndex)
                .Select(pair => pair.Value.Request);
        }

        public void CopyTo(Array array, int index)
        {
            foreach (var innerObject in _innerObjects)
            {
                array.SetValue(innerObject, index);
                index++;
            }
        }

        public void Add(TrackRequestDto item)
        {
            var requestGroup = _innerObjects
                .FirstOrDefault(group => group.Any(request => request.User.Id == item.User.Id));

            if (requestGroup == null)
            {
                var newGroup = new Queue<TrackRequestDto>();
                newGroup.Enqueue(item);
                _innerObjects.Enqueue(newGroup);
            }
            else
            {
                requestGroup.Enqueue(item);
                _count++;
            }
        }

        public TrackRequestDto GetNextRequest()
        {
            if (_innerObjects.Count > 0)
            {
                var nextRequestGroup = _innerObjects.Dequeue();
                if (nextRequestGroup.Count > 0)
                {
                    var track = nextRequestGroup.Dequeue();
                    if (nextRequestGroup.Count >= 1)
                    {
                        _innerObjects.Enqueue(nextRequestGroup);
                    }
                    else
                    {
                        _count--;
                    }
                    return track;
                }
            }

            _count = _innerObjects.Count;
            return null;
        }

        public int GetRequestsCountUser(string userId)
        {
            var requestGroup = _innerObjects
                .FirstOrDefault(group => group.Any(request => request.User.Id == userId));
            if (requestGroup == null)
            {
                return 0;
            }

            return requestGroup.Count;
        }

        public int Count => _count;
        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public List<TrackRequestDto> ToList()
        {
            return GetValues().ToList();
        }
    }
}