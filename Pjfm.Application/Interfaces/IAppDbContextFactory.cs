using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pjfm.Application.Interfaces
{
    public interface IAppDbContextFactory
    {
        DatabaseFacade CreateDatabase();
    }
}