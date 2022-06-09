using System.ComponentModel.DataAnnotations;

namespace WebSite.Models.User
{
    public class UserRegisterViewModel
    {
        [Display(Name = "First name: ")]
        [MaxLength(40)]
        public string? FirstName { get; set; }
        [Display(Name = "Last name: ")]
        [MaxLength(40)]
        public string? LastName { get; set; }

        [Display(Name = "Username: ")]
        [MaxLength(40)]
        [Required]
        public string UserName { get; set; }
        [Display(Name = "Password: ")]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [MaxLength(200)]
        [Required]
        public string Password { get; set; }
    }
}