using System.ComponentModel.DataAnnotations;

namespace Web.Api.Logic.Models.Account
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}