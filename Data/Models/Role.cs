using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Api.Data.Models
{
    public class Role : BaseEntity
    {
        [Required]
        public string Description { get; set; }

        public virtual IEnumerable<Permission> Permissions { get; set; }
        public virtual IEnumerable<User> Users { get; set; }
    }
}
