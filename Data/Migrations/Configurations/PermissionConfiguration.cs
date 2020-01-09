using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Api.Data.Models;

namespace Web.Api.Data.Migrations.Configurations
{
    public class PermissionConfiguration : BaseConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            // Foreign Key/s
            builder.HasOne(x => x.PermissionType).WithMany(x => x.Permissions).HasForeignKey(x => x.PermissionTypeId);
            builder.HasOne(x => x.Role).WithMany(x => x.Permissions).HasForeignKey(x => x.RoleId);
            builder.HasOne(x => x.User).WithMany(x => x.Permissions).HasForeignKey(x => x.UserId);

            // Column Constraints
            builder.Property(p => p.PermissionTypeId);
            builder.Property(p => p.RoleId);
            builder.Property(p => p.UserId);
            builder.Property(p => p.Enabled);
        }
    }
}
