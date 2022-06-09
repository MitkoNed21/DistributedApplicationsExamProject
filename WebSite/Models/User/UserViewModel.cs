using System.ComponentModel.DataAnnotations;

namespace WebSite.Models.User
{
    public class UserViewModel
    {
        [Display(Name = "Id: ")]
        public int Id { get; set; }

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

        [Display(Name = "Is admin: ")]
        [Required]
        public bool IsAdmin { get; set; }
    }
}
