using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Api.Data.Models
{
    public class PermissionType : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual IEnumerable<Permission> Permissions { get; set; }
    }
}
