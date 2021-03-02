using System.Collections.Generic;
using System.Linq;
using Pjfm.Application.Common.Dto;

namespace Pjfm.WebClient.Services
{
    public class PlaybackQueueSettings
    {
        public PlaybackQueueSettings()
        {
            TopTrackTermFilter = TopTrackTermFilter.AllTerms;
            BrowserQueueSettings = new BrowserQueueSettings()
            {
                Genres = new []{ "rock"},
                SeedTracks = new []{ "4u7EnebtmKWzUH433cf5Qv" },
                SeedArtists = new []{ "1dfeR4HaWDbWqFHLkxsg1d" },
                Tempo = QueueSettingsValue.Not,
                Instrumentalness = QueueSettingsValue.Not,
                Popularity = QueueSettingsValue.Not,
                DanceAbility = QueueSettingsValue.Not,
                Energy = QueueSettingsValue.Not,
                Valence = QueueSettingsValue.Not,
            };
        }
        
        private List<ApplicationUserDto> _includedUsers = new List<ApplicationUserDto>();
        public List<ApplicationUserDto> IncludedUsers
        {
            get => _includedUsers;
            set => _includedUsers = value;
        }

        public TopTrackTermFilter TopTrackTermFilter { get; set; }

        public BrowserQueueSettings BrowserQueueSettings { get; set; }

        public void AddIncludedUser(ApplicationUserDto user)
        {
            if (IncludedUsers.Select(x => x.Id).Contains(user.Id) == false)
            {
                _includedUsers.Add(user);
            }
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