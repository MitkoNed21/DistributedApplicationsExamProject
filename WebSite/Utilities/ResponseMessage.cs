namespace WebSite
{
    public class ResponseMessage
    {
        public string? Token { get; set; }

        public string? Message { get; set; }

        public object? Data { get; set; }

        public HttpResponseMessage? HttpResponseMessage { get; set; }

        public int? StatusCode => (int?)HttpResponseMessage?.StatusCode;

        public bool? IsSuccessStatusCode => (bool?)HttpResponseMessage?.IsSuccessStatusCode;
    }
}