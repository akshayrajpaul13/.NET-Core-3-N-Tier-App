using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Web.Api.Data.DbContexts
{
    public class AppDbFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        const string ConnectionString = "DatabaseConnection";

        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            //var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            AppDbContextConfigurer.Configure(builder, configuration.GetConnectionString(ConnectionString));

            return new AppDbContext(builder.Options);
        }
    }
}