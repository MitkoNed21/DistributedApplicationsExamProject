namespace WebSite.Models.MessageBoard
{
    public class IndexMessageBoardViewModel
    {
        public List<MessageBoardViewModel> MessageBoards { get; set; }

        public MessageBoardViewModel Filter { get; set; }
    }
}
