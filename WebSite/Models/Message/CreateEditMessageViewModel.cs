using System.ComponentModel.DataAnnotations;
using WebSite.Models.MessageBoard;

namespace WebSite.Models.Message
{
    public class CreateEditMessageViewModel
    {
        [Display(Name = "Id: ")]
        public int Id { get; set; }

        [Display(Name = "Title: ")]
        [MaxLength(50)]
        public string? Title { get; set; }

        [Display(Name = "Content: ")]
        [DataType(DataType.MultilineText)]
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }

        [Display(Name = "Is important: ")]
        [Required]
        public bool IsImportant { get; set; }

        [Display(Name = "Created by: ")]
        public int CreatedById { get; set; }
        [Display(Name = "Created by: ")]
        public string CreatedByUsername { get; set; }

        [Display(Name = "Message board: ")]
        [Required]
        public int MessageBoardId { get; set; }
        [Display(Name = "Message board: ")]
        public string MessageBoardName { get; set; }

        public List<MessageBoardViewModel> MessageBoards { get; set; }
    }
}
