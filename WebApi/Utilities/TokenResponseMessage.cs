using System.ComponentModel.DataAnnotations;

namespace WebApi.Utilities
{
    public class TokenResponseMessage : ResponseMessage
    {
        [Display(Name = "token")]
        public string Token { get; set; }

        public TokenResponseMessage(string message, string token) : base(message)
        {
            Token = token;
        }
    }
}
