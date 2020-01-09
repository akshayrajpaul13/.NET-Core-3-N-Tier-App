using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Api.Data.Models
{
    public class Session : BaseEntity
    {
        [Required]
        public string SessionGuid { get; set; }
        public int UserId { get; set; }
        public int TimeoutInMins { get; set; }
        public int LoginSourceId { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public virtual User User { get; set; }
    }
}
