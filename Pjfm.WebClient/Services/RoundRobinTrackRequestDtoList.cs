using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pjfm.Application.Common.Dto;
using pjfm.Models;

namespace Pjfm.WebClient.Services
{
    public class RoundRobinTrackRequestDtoList : ICollection
    {
        private Queue<Queue<TrackRequestDto>> _innerObjects = new Queue<Queue<TrackRequestDto>>();
        private int _count = 0;
        
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(_innerObjects.ToArray());
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
                    if (nextRequestGroup.Count > 1)
                        _innerObjects.Enqueue(nextRequestGroup);
                    return track;
                }
            }

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
    }

    public struct Enumerator : IEnumerator
    {
        private int _index;
        
        [AllowNull] private Queue<TrackRequestDto> _currentElement;
        private Queue<TrackRequestDto>[] _requestGroups;

        internal Enumerator(Queue<TrackRequestDto>[] requestGroups)
        {
            _requestGroups = requestGroups;
            _index = -1;
            _currentElement = default;
        }
        
        public bool MoveNext()
        {
            if (_index == -2)
                return false;

            _index++;

            if (_index == _requestGroups.Length)
            {
                _index = -2;
                _currentElement = default;
                return false;
            }

            _currentElement = _requestGroups[_index];
            return true;
        }

        public void Reset()
        {
            _index = -1;
            _currentElement = default;
        }

        public object Current
        {
            get
            {
                if (_index < 0)
                    ThrowEnumerationNotStartedOrEnded();
                return _currentElement;
            }
        }
        
        private void ThrowEnumerationNotStartedOrEnded()
        {
            Debug.Assert(_index == -1 || _index == -2);
            throw new InvalidOperationException();
        }
    }
}