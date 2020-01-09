using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Api.Data.Contracts;
using Web.Api.Data.Models;

namespace Web.Api.Data.Migrations.Configurations
{
    public class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity, IEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> entityTypeBuilder)
        {
            //Base Configurations
            entityTypeBuilder.Property(p => p.Id).UseIdentityColumn();
            entityTypeBuilder.Property(b => b.Created).HasDefaultValue(DateTime.UtcNow);
            entityTypeBuilder.Property(b => b.Modified).IsRequired(false);
        }
    }
}
