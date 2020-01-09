using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Api.Data.Models;

namespace Web.Api.Data.Migrations.Configurations
{
    public class TenantConfiguration : BaseConfiguration<Tenant>
    {
        public override void Configure(EntityTypeBuilder<Tenant> builder)
        {
            // Foreign Key/s

            // Column Constraints
            builder.Property(p => p.Code);
            builder.Property(p => p.Name);
            builder.Property(p => p.Description);
        }
    }
}
