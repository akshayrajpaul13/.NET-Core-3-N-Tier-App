using System.Collections.Generic;

namespace Web.Api.Data.Models
{
    public class Permission : BaseEntity
    {
        public int PermissionTypeId { get; set; }
        public int? RoleId { get; set; }
        public int? UserId { get; set; }
        public bool Enabled { get; set; }

        public virtual PermissionType PermissionType { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
