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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class MessagesController : ControllerBase
    {
        private readonly MessageManagementService service;
        private readonly UserManagementService userService;
        private readonly MessageBoardManagementService messageBoardService;

        public MessagesController(
            MessageManagementService service,
            UserManagementService userService,
            MessageBoardManagementService messageBoardService)
        {
            this.service = service;
            this.userService = userService;
            this.messageBoardService = messageBoardService;
        }

        // GET: api/<MessagesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await service.GetAsync();
            return Ok(results);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFiltered(MessageDto filterDto)
        {
            List<MessageDto> results;
            if (filterDto is not null && filterDto.Content is not null)
            {
                results = await service.GetFilteredAsync(m => m.Content.Contains(filterDto.Content));
                return Ok(results);
            }

            results = await service.GetAsync();
            return Ok(results);
        }

        // GET api/<MessagesController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var message = await service.GetByIdAsync(id);

            if (message is null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // POST api/<MessagesController>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(MessageDto messageDto)
        {
            var currentUserId = int.Parse(HttpContext.User.FindFirst("id")!.Value);
            messageDto.CreatedById = currentUserId;

            if (!messageDto.Validate()) return BadRequest(new ResponseMessage("The data provided is not valid!"));

            var messageBoard = await messageBoardService.GetByIdAsync(messageDto.MessageBoardId);

            if (messageBoard is null || !messageBoard.IsOpen)
            {
                return BadRequest(new ResponseMessage("The message board data provided is not valid!"));
            }

            if (await service.SaveAsync(messageDto))
            {
                return Ok(new ResponseMessage("Message was saved!"));
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseMessage("Message was not saved!")
                );
            }
        }

        // PUT api/<MessagesController>/5
        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(MessageDto messageDto)
        {
            if (!messageDto.Validate()) return BadRequest(new ResponseMessage("The data provided is not valid!"));

            var currentUserId = int.Parse(HttpContext.User.FindFirst("id")!.Value);
            var isAdmin = bool.Parse(HttpContext.User.FindFirst("isAdmin")!.Value);

            if (messageDto.CreatedById != currentUserId && !isAdmin)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ResponseMessage("User is unauthorized to update this message!")
                );
            }

            var messageBoard = await messageBoardService.GetByIdAsync(messageDto.MessageBoardId);
            if (messageBoard is null || !messageBoard.IsOpen)
            {
                return BadRequest(new ResponseMessage("The message board data provided is not valid!"));
            }

            if (await service.UpdateAsync(messageDto))
            {
                return Ok(new ResponseMessage("Message was updated!"));
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseMessage("Message was not updated!")
                );
            }
        }

        // DELETE api/<MessagesController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await service.GetByIdAsync(id);
            if (message is null) return NotFound();

            var messageBoard = await messageBoardService.GetByIdAsync(message.MessageBoardId);
            if (messageBoard is null || !messageBoard.IsOpen)
            {
                return BadRequest(new ResponseMessage("The message board data provided is not valid!"));
            }

            var currentUserId = int.Parse(HttpContext.User.FindFirst("id")!.Value);
            var isAdmin = bool.Parse(HttpContext.User.FindFirst("isAdmin")!.Value);

            if (message.CreatedById != currentUserId && messageBoard.CreatedById != currentUserId && !isAdmin)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ResponseMessage("User is unauthorized to delete this message!")
                );
            }

            if (await service.DeleteAsync(id))
            {
                return Ok(new ResponseMessage("Message was deleted!"));
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseMessage("Message was not deleted!")
                );
            }
        }
    }
}
