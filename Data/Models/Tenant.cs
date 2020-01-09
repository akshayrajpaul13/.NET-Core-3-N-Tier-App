using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Api.Data.Models
{
    public class Tenant : BaseEntity
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public IEnumerable<User> Users { get; set; }
    }
}
