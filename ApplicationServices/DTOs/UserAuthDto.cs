namespace ApplicationServices.DTOs
{
    public class UserAuthDto : BaseDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public override bool Validate()
        {
            return
                !String.IsNullOrWhiteSpace(UserName) && UserName.Length <= 40 &&
                !String.IsNullOrWhiteSpace(Password) && Password.Length is >= 8 and <= 200;
        }
    }
}