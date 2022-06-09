using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        
        public virtual int? CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; }

        public virtual int? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
