using System.ComponentModel.DataAnnotations;

namespace Web.Api.Logic.Models.Account
{
    public interface ILoginModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        string Password { get; set; }

        [Display(Name = "Remember me?")]
        bool RememberMe { get; set; }
    }
}