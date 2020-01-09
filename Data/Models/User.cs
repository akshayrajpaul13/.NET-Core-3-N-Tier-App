using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Api.Data.Models
{
    public class User : BaseEntity
    {
        public int? TenantId { get; set; }
        public int RoleId { get; set; }
        [Required]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string VerificationCode { get; set; }
        public DateTime? ConfirmEmailExpiry { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public int AccessFailedCount { get; set; }
        public bool ForcePasswordReset { get; set; }
        public string FacebookId { get; set; }
        public string GoogleId { get; set; }
        public string MicrosoftId { get; set; }
        public int LoginTypeId { get; set; }
        public int SourceId { get; set; }
        public string AuthenticationToken { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual Role Role { get; set; }
        public virtual IEnumerable<Session> Sessions { get; set; }
        public virtual IEnumerable<Permission> Permissions { get; set; }

        //public virtual ICollection<Contact> Contacts { get; set; }
        //public virtual ICollection<Address> Addresses { get; set; }
    }
}
