using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Api.Data.Models;

namespace Web.Api.Data.Migrations.Configurations
{
    public class PermissionTypeConfiguration : BaseConfiguration<PermissionType>
    {
        public override void Configure(EntityTypeBuilder<PermissionType> builder)
        {
            // Foreign Key/s
            builder.HasMany(x => x.Permissions).WithOne(x => x.PermissionType).HasForeignKey(x => x.PermissionTypeId);

            // Column Constraints
            builder.Property(p => p.Name);
            builder.Property(p => p.Description);
        }
    }
}
