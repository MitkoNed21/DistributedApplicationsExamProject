using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Message : BaseEntity
    {
        [MaxLength(50)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }

        [Required]
        public bool IsImportant { get; set; }


        [Required]
        public override int? CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }


        public int MessageBoardId { get; set; }
        public virtual MessageBoard MessageBoard { get; set; }
    }
}
