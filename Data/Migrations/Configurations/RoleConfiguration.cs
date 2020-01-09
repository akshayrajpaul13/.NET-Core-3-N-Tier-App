using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Api.Data.Models;

namespace Web.Api.Data.Migrations.Configurations
{
    public class RoleConfiguration : BaseConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            // Foreign Keys
            builder.HasMany(x => x.Permissions).WithOne(x => x.Role).HasForeignKey(x => x.RoleId);

            // Column Constraints
            builder.Property(p => p.Description);
        }
    }
}
