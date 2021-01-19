using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Pjfm.Application.Interfaces;
using Pjfm.Domain.Interfaces;
using Pjfm.Infrastructure.Persistence;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Pjfm.Infrastructure.Service
{
    public class AppDbContextFactory : IAppDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public AppDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        
        public DatabaseFacade CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            
            optionsBuilder.UseMySql(_configuration["ConnectionStrings:ApplicationDb"],
                builder =>
                {
                    builder.MigrationsAssembly("Pjfm.WebClient");
                    builder.ServerVersion(new ServerVersion(new Version(10, 3, 25), ServerType.MariaDb));
                });
            var context = new AppDbContext(optionsBuilder.Options);
            return context.Database;
        }
    }
}