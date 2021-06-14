using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Interfaces;
using Pjfm.Infrastructure.Persistence;

namespace Pjfm.Infrastructure.Service
{
    public class DatabaseFactory : IAppDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public DatabaseFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        
        public DatabaseFacade CreateDatabase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var connectionString = _configuration["ConnectionStrings:ApplicationDb"];
            optionsBuilder.UseSqlServer(new SqlConnection(connectionString),
                builder =>
                {
                    builder.MigrationsAssembly("Pjfm.Api");
                });
            var context = new AppDbContext(optionsBuilder.Options);
            return context.Database;
        }
    }
}