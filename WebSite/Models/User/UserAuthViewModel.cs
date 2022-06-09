using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebSite.Models.User
{
    public class UserAuthViewModel
    {
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
