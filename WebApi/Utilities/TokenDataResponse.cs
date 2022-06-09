using System.ComponentModel.DataAnnotations;

namespace WebApi.Utilities
{
    public class TokenDataResponse<T> : ResponseMessage
    {
        [Display(Name = "data")]
        public T Data { get; set; }

        [Display(Name = "token")]
        public string Token { get; set; }

        public TokenDataResponse(T data, string token) : base(null)
        {
            Data = data;
            Token = token;
        }

    }
}
