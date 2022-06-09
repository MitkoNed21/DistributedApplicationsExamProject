namespace WebSite.Models.User
{
    public class IndexUserViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();
        public UserViewModel Filter { get; set; }
    }
}
