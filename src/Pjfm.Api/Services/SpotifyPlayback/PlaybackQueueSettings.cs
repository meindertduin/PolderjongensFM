using System.Collections.Generic;
using System.Linq;
using Pjfm.Application.Common.Dto;

namespace Pjfm.WebClient.Services
{
    public class PlaybackQueueSettings
    {
        private List<ApplicationUserDto> _includedUsers = new List<ApplicationUserDto>();
        public List<ApplicationUserDto> IncludedUsers
        {
            get => _includedUsers;
            set => _includedUsers = value;
        }

        private TopTrackTermFilter _topTrackTermFilter;

        public TopTrackTermFilter TopTrackTermFilter
        {
            get => _topTrackTermFilter;
            set => _topTrackTermFilter = value;
        }

        public void AddIncludedUser(ApplicationUserDto user)
        {
            _includedUsers.Add(user);
        }

        public bool TryRemoveIncludedUser(ApplicationUserDto user)
        {
            var item = IncludedUsers.SingleOrDefault(x => x.Id == user.Id);
            if (item != null)
            {
                IncludedUsers.Remove(item);
                return true;
            }

            return false;
        }
    }
}