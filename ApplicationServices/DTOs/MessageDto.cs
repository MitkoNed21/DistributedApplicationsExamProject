using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.DTOs
{
    public class MessageDto : BaseDto
    {
        public string? Title { get; set; }

        public string Content { get; set; }

        public bool IsImportant { get; set; }

        public int CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }

        public int MessageBoardId { get; set; }

        public override bool Validate()
        {
            return
                (String.IsNullOrWhiteSpace(Title) || Title.Length <= 50) &&
                !String.IsNullOrWhiteSpace(Content) && Content.Length <= 2000;
        }
    }
}
