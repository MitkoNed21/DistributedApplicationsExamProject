using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/version")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public const string VERSION_STRING = $"MessageBoards Web API v{VERSION}";
        public const string VERSION = "1.0";

        [HttpGet("/api/version")]
        public IActionResult Version()
        {
            return Ok(new
            {
                versionString = VERSION_STRING,
                version = VERSION
            });
        }
    }
}
