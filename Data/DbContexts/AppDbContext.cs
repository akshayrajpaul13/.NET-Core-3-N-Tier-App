using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Web.Api.Data.Contracts;
using Web.Api.Data.Migrations.Configurations;
using Web.Api.Data.Models;

namespace Web.Api.Data.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        #region DbSets
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }
        public DbSet<Role> Roles { get; set; }
        #endregion

        public static AppDbContext Create()
        {
            return new AppDbContext(new DbContextOptions<AppDbContext>());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TenantConfiguration());
            modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new SessionConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }

        /// <summary>
        /// Saves the without audit. This was only created for tests. Please do not use with production code
        /// </summary>
        /// <returns></returns>
        public int SaveWithoutAudit()
        {
            return base.SaveChanges();
        }

        public override int SaveChanges()
        {
            SetAuditInfo();

            return base.SaveChanges();
        }

        public void SetAuditInfo()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is IEntity && (e.State == EntityState.Added || e.State == EntityState.Modified)))
            {
                var now = DateTime.UtcNow;

                var e = (IEntity)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    e.Created = now;
                }
                e.Modified = now;
            }
        }
    }
}
