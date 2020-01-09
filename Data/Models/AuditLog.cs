using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Api.Data.Models
{
    public class AuditLog : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int Id { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public DateTime Date { get; set; }
        public int ReferenceId { get; set; }
        [Required]
        public string ReferenceType { get; set; }
        [Required]
        public string Change { get; set; }
        public string FieldName { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        [NotMapped]
        public new DateTime Created { get; set; }
        [NotMapped]
        public new DateTime Modified { get; set; }

    }
}
