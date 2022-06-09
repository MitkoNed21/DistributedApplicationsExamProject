using System.ComponentModel.DataAnnotations;

namespace WebApi.Utilities
{
    public class ResponseMessage
    {
        [Display(Name = "message")]
        public string Message { get; set; }

        public ResponseMessage(string message)
        {
            Message = message;
        }
    }
}
