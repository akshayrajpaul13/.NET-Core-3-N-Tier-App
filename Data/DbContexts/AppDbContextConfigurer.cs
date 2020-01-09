using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Data.DbContexts
{
    public static class AppDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AppDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AppDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}