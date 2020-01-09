using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Api.Data.Models;

namespace Web.Api.Data.Migrations.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public virtual void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            //Foreign Key(s)

            // Column Constraints
            builder.Property(p => p.Id);
            builder.Property(p => p.TenantId);
            builder.Property(p => p.UserId);
            builder.Property(p => p.Date);
            builder.Property(p => p.ReferenceId);
            builder.Property(p => p.ReferenceType);
            builder.Property(p => p.Change);
            builder.Property(p => p.FieldName).IsRequired(false);
            builder.Property(p => p.From).IsRequired(false);
            builder.Property(p => p.To).IsRequired(false);
        }
    }
}
