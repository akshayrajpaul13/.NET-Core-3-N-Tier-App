using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Api.Data.Models;

namespace Web.Api.Data.Migrations.Configurations
{
    public class UserConfiguration : BaseConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            // Foreign Key/s
            builder.HasOne(x => x.Tenant).WithMany(x => x.Users).HasForeignKey(x => x.TenantId);
            builder.HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleId);
            builder.HasMany(p => p.Sessions).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            //HasMany(p => p.Contacts).WithRequired(x => x.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            //HasMany(p => p.Addresses).WithRequired(x => x.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);

            // Column Constraints
            builder.Property(p => p.TenantId);
            builder.Property(p => p.RoleId);
            builder.Property(p => p.Email);
            builder.Property(p => p.EmailConfirmed);
            builder.Property(p => p.PasswordHash);
            builder.Property(p => p.VerificationCode);
            builder.Property(p => p.ConfirmEmailExpiry);
            builder.Property(p => p.LockoutEndDateUtc);
            builder.Property(p => p.AccessFailedCount);
            builder.Property(p => p.ForcePasswordReset);
        }
    }
}
