using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class MessageBoard : BaseEntity
    {
        [Required]
        [MaxLength(70)]
        public string Name { get; set; }

        [Required]
        public bool IsOpen { get; set; }

        [Required]
        public override int? CreatedById { get; set; }
        [InverseProperty(nameof(User.MessageBoards))]
        public virtual User CreatedBy { get; set; }

        [Required]
        public override int? UpdatedById { get; set; }
        [ForeignKey(nameof(UpdatedById))]
        public virtual User UpdatedBy { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
