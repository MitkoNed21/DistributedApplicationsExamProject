using System.ComponentModel.DataAnnotations;

namespace WebSite.Models.Message
{
    public class MessageViewModel
    {
        [Display(Name = "Id: ")]
        public int Id { get; set; }

        [Display(Name = "Title: ")]
        [MaxLength(50)]
        public string? Title { get; set; }

        [Display(Name = "Content: ")]
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }

        [Display(Name= "Is important: ")]
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
    }
}
