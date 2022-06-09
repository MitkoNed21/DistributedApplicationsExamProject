using System.ComponentModel.DataAnnotations;

namespace WebSite.Models.MessageBoard
{
    public class MessageBoardViewModel
    {
        [Display(Name = "Id: ")]
        public int Id { get; set; }

        [Display(Name = "Name: ")]
        [Required]
        [MaxLength(70)]
        public string Name { get; set; }

        [Display(Name = "Is open: ")]
        [Required]
        public bool IsOpen { get; set; }

        [Display(Name = "Created by: ")]
        public int CreatedById { get; set; }
        [Display(Name = "Created by: ")]
        public string CreatedByUsername { get; set; }

        [Display(Name = "Updated by: ")]
        public int UpdatedById { get; set; }
        [Display(Name = "Updated by: ")]
        public string UpdatedByUsername { get; set; }
        [Display(Name = "Updated on: ")]
        public DateTime UpdatedOn { get; set; }

        [Display(Name = "Messages: ")]
        public List<Models.Message.MessageViewModel> Messages { get; set; } = new();
    }
}
