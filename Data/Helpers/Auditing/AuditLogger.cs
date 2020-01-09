using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Web.Api.Data.Contracts;
using Web.Api.Data.DbContexts;
using Web.Api.Data.Enums;
using Web.Api.Data.Models;

namespace Web.Api.Data.Helpers.Auditing
{
    /// <summary>
    /// Provides automatic audit logging. Writes one db row per field that has changed, which is more friendly
    /// for querying on the audit log table, and is easier for following up how individual fields have changed
    /// over time.
    /// </summary>
    public class AuditLogger
    {
        private readonly AppDbContext _context;
        private readonly int? _userId;
        private readonly Dictionary<int, List<AuditLog>> _auditLogsPerEntity; //index position, audit logs per object
        private readonly DateTime _now;

        public AuditLogger(AppDbContext context, int? userId)
        {
            _context = context;
            _userId = userId;
            _auditLogsPerEntity = new Dictionary<int, List<AuditLog>>();
            _now = DateTime.UtcNow;
        }

        /// <summary>
        /// Prepares auditing for entities about to be created.
        /// This has to be split into two steps, because the object Ids do not exist yet, but after
        /// the db commit, the change tracking will have been cleared.
        /// Call this before committing to the db, and then afterwards, call AuditCreatedAfter.
        /// </summary>
        public void AuditCreatedBefore(List<IEntity> entities)
        {
            // Stores all the audit logs per entity. Use index position because we don't yet have an Id to use - this
            // relies on exactly the same set of entities being supplied on the "After" call.
            foreach (var entity in entities)
            {
                _auditLogsPerEntity.Add(entities.IndexOf(entity), GetAuditLogsForCreate(entity));
            }
        }

        private List<AuditLog> GetAuditLogsForCreate(IEntity entity)
        {
            // Check if this entity is being audited
            var entityName = entity.GetType().Name;
            if (IsEntityExcludedFromAuditing(entityName))
                return new List<AuditLog>();

            // Get all the change records for this entity
            var addedEntities = _context.ChangeTracker.Entries().Where(x => x.Entity == entity && x.State == EntityState.Added).ToList();
            var auditLogs = new List<AuditLog>();
            foreach (var addedEntity in addedEntities)
            {
                // Write one row per field that has changed
                foreach (var fieldName in addedEntity.CurrentValues.Properties)
                {
                    // Check if this field is being audited
                    if (IsFieldNameExcludedFromAuditing(entityName, fieldName.Name))
                        continue;

                    // Don't log null values
                    var currentValue = addedEntity.CurrentValues[fieldName.Name]?.ToString();
                    if (currentValue == null)
                        continue;

                    auditLogs.Add(new AuditLog
                    {
                        TenantId = null, //TODO ES 2016-12-13: find a way to retrieve this data reliably
                        UserId = _userId,
                        Date = _now,
                        ReferenceId = entity.Id,
                        ReferenceType = entityName,
                        Change = ChangeTypes.Created.ToString(),
                        FieldName = fieldName.Name,
                        From = null,
                        To = currentValue,
                    });
                }
            }

            return auditLogs;
        }

        /// <summary>
        /// Completes auditing for created entities.  Call this after AuditCreatedBefore and after committing to the db, and ensure
        /// that exactly the same list of entities is passed in.
        /// </summary>
        public void AuditCreatedAfter(List<IEntity> entities)
        {
            var auditLogs = new List<AuditLog>();

            // Set the reference id on all the objects, now that it's available.
            // Combine all audit logs so there is only one call to the db.
            foreach (var entity in entities)
            {
                var aLogs = _auditLogsPerEntity[entities.IndexOf(entity)];
                if (aLogs.Any())
                {
                    foreach (var auditLog in aLogs)
                    {
                        auditLog.ReferenceId = entity.Id;
                    }

                    auditLogs.AddRange(aLogs);
                }
            }

            if (auditLogs.Any())
            {
                _context.AuditLogs.AddRange(auditLogs);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Audit entities that are about to be updated.  Call this before committing to the db.
        /// </summary>
        public void AuditUpdated(List<IEntity> entities)
        {
            // Combine all audit logs so there is only one call to the db.
            var auditLogs = new List<AuditLog>();
            foreach (var entity in entities)
            {
                auditLogs.AddRange(GetAuditLogsForUpdate(entity));
            }

            if (auditLogs.Any())
            {
                _context.AuditLogs.AddRange(auditLogs);
                //context.SaveChanges(); // done in repo, reducing the number of db calls
            }
        }

        private List<AuditLog> GetAuditLogsForUpdate(IEntity entity)
        {
            // Check if this entity is being audited
            var entityName = entity.GetType().Name;
            if (IsEntityExcludedFromAuditing(entityName))
                return new List<AuditLog>();

            // Get all the change records for this entity
            var updatedEntities = _context.ChangeTracker.Entries().Where(x => x.Entity == entity && x.State == EntityState.Modified).ToList();
            var auditLogs = new List<AuditLog>();
            foreach (var updatedEntity in updatedEntities)
            {
                // Write one row per field that has changed
                foreach (var prop in updatedEntity.OriginalValues.Properties)
                {
                    // Check if this field is being audited
                    if (IsFieldNameExcludedFromAuditing(entityName, prop.Name))
                        continue;

                    var originalValue = updatedEntity.OriginalValues[prop]?.ToString();
                    var currentValue = updatedEntity.CurrentValues[prop]?.ToString();

                    // Don't log fields that haven't changed
                    if (originalValue == currentValue)
                        continue;

                    auditLogs.Add(new AuditLog
                    {
                        TenantId = null, //TODO ES 2016-12-13: find a way to retrieve this data reliably
                        UserId = _userId,
                        Date = _now,
                        ReferenceId = entity.Id,
                        ReferenceType = entityName,
                        Change = ChangeTypes.Updated.ToString(),
                        FieldName = prop.Name,
                        From = originalValue,
                        To = currentValue,
                    });
                }
            }

            return auditLogs;
        }

        /// <summary>
        /// Audit entities that are about to be deleted.  Call this before committing to the db.
        /// </summary>
        public void AuditDeleted(List<IEntity> entities)
        {
            // Combine all audit logs so there is only one call to the db.
            var auditLogs = new List<AuditLog>();
            foreach (var entity in entities)
            {
                auditLogs.AddRange(GetAuditLogsForDelete(entity));
            }

            if (auditLogs.Any())
            {
                _context.AuditLogs.AddRange(auditLogs);
                //context.SaveChanges(); // done in repo, reducing the number of db calls
            }
        }

        private List<AuditLog> GetAuditLogsForDelete(IEntity entity)
        {
            // Check if this entity is being audited
            var entityName = entity.GetType().Name;
            if (IsEntityExcludedFromAuditing(entityName))
                return new List<AuditLog>();

            // Get all the change records for this entity
            var deletedEntities = _context.ChangeTracker.Entries().Where(x => x.Entity == entity && x.State == EntityState.Deleted).ToList();
            var auditLogs = new List<AuditLog>();
            foreach (var deletedEntity in deletedEntities)
            {
                // Write one row per field that has changed
                foreach (var prop in deletedEntity.OriginalValues.Properties)
                {
                    // Check if this field is being audited
                    if (IsFieldNameExcludedFromAuditing(entityName, prop.Name))
                        continue;

                    // Don't log fields that are null
                    var currentValue = deletedEntity.OriginalValues[prop]?.ToString();
                    if (currentValue == null)
                        continue;

                    auditLogs.Add(new AuditLog
                    {
                        TenantId = null, //TODO ES 2016-12-13: find a way to retrieve this data reliably
                        UserId = _userId,
                        Date = _now,
                        ReferenceId = entity.Id,
                        ReferenceType = entityName,
                        Change = ChangeTypes.Deleted.ToString(),
                        FieldName = prop.Name,
                        From = currentValue,
                        To = null,
                    });
                }
            }

            return auditLogs;
        }

        /// <summary>
        /// Indicates whether an entity should be excluded from auditing.  Especially ignore entities
        /// that represent some kind of logging, or entites for which there is little value
        /// in providing auditing.
        /// </summary>
        public bool IsEntityExcludedFromAuditing(string name)
        {
            var excludedTypes = new List<Type>
            {
                typeof(AuditLog)
            };

            var excludedTypeNames = excludedTypes.Select(x => x.Name);
            return excludedTypeNames.Contains(name);
        }

        /// <summary>
        /// Indicates whether a common field name should be excluded from auditing, since there is
        /// little value in logging the field.
        /// </summary>
        public bool IsFieldNameExcludedFromAuditing(string entityName, string fieldName)
        {
            if (IsGenericFieldNameExcluded(fieldName))
                return true;

            return IsClassFieldNameExcluded(entityName, fieldName);
        }

        private static bool IsGenericFieldNameExcluded(string fieldName)
        {
            var excludedFieldNames = new List<string>
            {
                "Id", // available via ReferenceId
                "Created", // available via Date
                "Modified", // available via Date
                "SessionId", // on user profile
                "SessionGuid", // on user profile
                "SessionEnd", // on user profile
                "PasswordHash", // on user, sensitive info
                "VerificationCode", // on user, sensitive info
            };

            return excludedFieldNames.Contains(fieldName);
        }

        private static bool IsClassFieldNameExcluded(string entityName, string fieldName)
        {
            var excludedFieldNames = new Dictionary<string, List<string>>
            {
                //{ ReferenceTypes.Setting.ToString(), new List<string> { "Required", "Type" }},
                { ReferenceTypes.Permission.ToString(), new List<string> { "UserId", "PermissionTypeId" }},
            };

            if (!excludedFieldNames.ContainsKey(entityName))
                return false;

            return excludedFieldNames[entityName].Contains(fieldName);
        }
    }
}
