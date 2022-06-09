using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    [Index(nameof(UserName), IsUnique = true)]
    public class User : BaseEntity
    {
        [MaxLength(40)]
        public string? FirstName { get; set; }
        [MaxLength(40)]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(40)]
        public string UserName { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(200)]
        public string Password { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        public virtual ICollection<MessageBoard> MessageBoards { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

    }
}
