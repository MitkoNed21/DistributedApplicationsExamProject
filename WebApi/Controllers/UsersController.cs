using ApplicationServices.DTOs;
using ApplicationServices.Implementations;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using WebApi.ActionFilters;
using WebApi.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class UsersController : ControllerBase
    {
        private readonly UserManagementService service;

        public UsersController(UserManagementService service)
        {
            this.service = service;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await service.GetAsync();
            return Ok(results);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFiltered(UserDto filterDto)
        {
            List<UserDto> results;
            if (filterDto is not null && filterDto.UserName is not null)
            {
                results = await service.GetFilteredAsync(u => u.UserName.Contains(filterDto.UserName));
                return Ok(results);
            }

            results = await service.GetAsync();
            return Ok(results);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var user = await service.GetByIdAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT api/<UsersController>
        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(UserDto userDto)
        {
            if (!userDto.Validate()) return BadRequest(new ResponseMessage("The data provided is not valid!"));

            var currentUserId = int.Parse(HttpContext.User.FindFirst("id")!.Value);
            //var isAdmin = bool.Parse(HttpContext.User.FindFirst("isAdmin")!.Value);

            if (userDto.Id != currentUserId)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ResponseMessage("User is unauthorized to update this user!")
                );
            }

            if (await service.UpdateAsync(userDto))
            {
                return Ok(new ResponseMessage("User was updated!"));
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseMessage("User was not updated!")
                );
            }
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = int.Parse(HttpContext.User.FindFirst("id")!.Value);
            var isAdmin = bool.Parse(HttpContext.User.FindFirst("isAdmin")!.Value);

            if (id != currentUserId && !isAdmin)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ResponseMessage("User is unauthorized to delete this user!")
                );
            }

            if (await service.DeleteAsync(id))
            {
                return Ok(new ResponseMessage("User was deleted!"));
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseMessage("User was not deleted!")
                );
            }
        }

        //TODO:
        // PUT api/<UsersController>/promote/5
        [HttpPut("promote/{id}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Promote(int id)
        {
            var isAdmin = bool.Parse(HttpContext.User.FindFirst("isAdmin")!.Value);

            if (!isAdmin)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ResponseMessage("Only admins can promote!")
                );
            }

            if (await service.PromoteAdminAsync(id))
            {
                return Ok(new ResponseMessage("User was promoted to admin!"));
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseMessage("User was not promoted to admin!")
                );
            }
        }

        // PUT api/<UsersController>/demote/5
        [HttpPut("demote/{id}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Demote(int id)
        {
            var isAdmin = bool.Parse(HttpContext.User.FindFirst("isAdmin")!.Value);

            if (!isAdmin)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ResponseMessage("Only admins can demote!")
                );
            }

            if (await service.DemoteAdminAsync(id))
            {
                return Ok(new ResponseMessage("User was demoted!"));
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseMessage("User was not demoted!")
                );
            }
        }
    }
}
