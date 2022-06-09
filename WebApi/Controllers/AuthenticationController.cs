using ApplicationServices.DTOs;
using ApplicationServices.Implementations;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using WebApi.ActionFilters;
using WebApi.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationManagementService service;

        public AuthenticationController(AuthenticationManagementService service)
        {
            this.service = service;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            if (!userRegisterDto.Validate()) return BadRequest(new ResponseMessage("The data provided is not valid!"));

            if (await service.GetByUserNameAsync(userRegisterDto.UserName) is not null)
            {
                return BadRequest(new ResponseMessage("User with such username already exists!"));
            }

            if (await service.SaveAsync(userRegisterDto))
            {
                return Ok(new ResponseMessage("User created!"));
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseMessage("User was not created!")
                );
            }
        }

        [HttpPost("login")]
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Authenticate(UserAuthDto userAuthDto)
        {
            if (!userAuthDto.Validate()) return BadRequest(new ResponseMessage("The data provided is not valid!"));

            var token = await service.AuthenticateAsync(userAuthDto);

            if (token is null)
            {   
                return BadRequest(new ResponseMessage("Authentication request failed!"));
            }
            else
            {
                return Ok(new TokenResponseMessage("Authentication succeeded!", token));
            }
        }

        [HttpPost("loggedInInfo")]
        [Authorize]
        public async Task<IActionResult> GetLoggedUser()
        {
            var currentUserId = int.Parse(HttpContext.User.FindFirst("id")!.Value);
            var firstName = HttpContext.User.FindFirst("firstName")!.Value;
            var lastName = HttpContext.User.FindFirst("lastName")!.Value;
            var userName = HttpContext.User.FindFirst("userName")!.Value;
            var isAdmin = bool.Parse(HttpContext.User.FindFirst("isAdmin")!.Value);

            return Ok(new UserDto()
            {
                Id = currentUserId,
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                IsAdmin = isAdmin
            });
        }
    }
}
