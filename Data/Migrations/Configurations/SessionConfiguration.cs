using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Api.Data.Models;

namespace Web.Api.Data.Migrations.Configurations
{
    public class SessionConfiguration : BaseConfiguration<Session>
    {
        public override void Configure(EntityTypeBuilder<Session> builder)
        {
            // Foreign Key/s
            builder.HasOne(p => p.User).WithMany(x => x.Sessions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);

            // Column Constraints
            builder.Property(p => p.SessionGuid);
            builder.Property(p => p.UserId);
            builder.Property(p => p.LoginSourceId);
            builder.Property(p => p.TimeoutInMins);
            builder.Property(p => p.Start);
            builder.Property(p => p.End);
        }
    }
}
