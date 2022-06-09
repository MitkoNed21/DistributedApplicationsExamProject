using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.DTOs
{
    public class MessageBoardDto : BaseDto
    {
        public string Name { get; set; }

        public bool IsOpen { get; set; }

        public int CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }

        public int UpdatedById { get; set; }
        public DateTime UpdatedOn { get; set; }

        public override bool Validate()
        {
            return
                !String.IsNullOrWhiteSpace(Name) && Name.Length <= 70;
        }
    }
}
