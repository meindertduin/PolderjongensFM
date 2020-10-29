using System.Collections.Generic;
using System.Threading.Tasks;
using Pjfm.Domain.Entities;

namespace Pjfm.Domain.Interfaces
{
    public interface IRetrieveStrategy
    {
        public Task<List<TopTrack>> RetrieveItems(string accessToken, int term, string userId);
    }
}