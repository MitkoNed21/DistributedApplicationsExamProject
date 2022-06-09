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
    public class MessageBoardsController : ControllerBase
    {
        private readonly MessageBoardManagementService service;

        public MessageBoardsController(MessageBoardManagementService service)
        {
            this.service = service;
        }

        // GET: api/<MessageBoardsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await service.GetAsync();
            return Ok(results);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFiltered(MessageBoardDto filterDto)
        {
            List<MessageBoardDto> results;
            if (filterDto is not null && filterDto.Name is not null)
            {
                results = await service.GetFilteredAsync(m => m.Name.Contains(filterDto.Name));
                return Ok(results);
            }

            results = await service.GetAsync();
            return Ok(results);
        }

        // GET api/<MessageBoardsController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var messageBoard = await service.GetByIdAsync(id);

            if (messageBoard is null)
            {
                return NotFound();
            }

            return Ok(messageBoard);
        }

        [HttpGet("{id}/messages")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMessages(int id)
        {
            var messageBoard = await service.GetByIdAsync(id);

            if (messageBoard is null)
            {
                return NotFound();
            }

            var message = await service.GetMessagesAsync(id);

            return Ok(message);
        }

        [HttpGet("{id}/messages/filtered")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMessagesFiltered(int id, MessageDto filterDto)
        {
            var messageBoard = await service.GetByIdAsync(id);

            if (messageBoard is null)
            {
                return NotFound();
            }

            List<MessageDto> results;
            if (filterDto is not null && filterDto.Content is not null)
            {
                results = await service.GetMessagesFilteredAsync(id, m => m.Content.Contains(filterDto.Content));
                return Ok(results);
            }

            var messages = await service.GetMessagesAsync(id);
            return Ok(messages);
        }

        // POST api/<MessageBoardsController>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(MessageBoardDto messageBoardDto)
        {
            var currentUserId = int.Parse(HttpContext.User.FindFirst("id")!.Value);
            messageBoardDto.UpdatedById = messageBoardDto.CreatedById = currentUserId;
            messageBoardDto.IsOpen = true;

            if (!messageBoardDto.Validate()) return BadRequest(new ResponseMessage("The data provided is not valid!"));

            if (await service.SaveAsync(messageBoardDto))
            {
                return Ok(new ResponseMessage("MessageBoard was saved!"));
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseMessage("MessageBoard was not saved!")
                );
            }
        }

        // PUT api/<MessageBoardsController>/5
        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(MessageBoardDto messageBoardDto)
        {
            var currentUserId = int.Parse(HttpContext.User.FindFirst("id")!.Value);
            messageBoardDto.UpdatedById = currentUserId;

            if (!messageBoardDto.Validate()) return BadRequest(new ResponseMessage("The data provided is not valid!"));

            var isAdmin = bool.Parse(HttpContext.User.FindFirst("isAdmin")!.Value);

            if (messageBoardDto.CreatedById != currentUserId && !isAdmin)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ResponseMessage("User is unauthorized to update this message board!")
                );
            }

            if (await service.UpdateAsync(messageBoardDto))
            {
                return Ok(new ResponseMessage("MessageBoard was updated!"));
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseMessage("MessageBoard was not updated!")
                );
            }
        }

        // DELETE api/<MessageBoardsController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var messageBoard = await service.GetByIdAsync(id);
            if (messageBoard is null) return NotFound();

            var currentUserId = int.Parse(HttpContext.User.FindFirst("id")!.Value);
            var isAdmin = bool.Parse(HttpContext.User.FindFirst("isAdmin")!.Value);

            if (messageBoard.CreatedById != currentUserId && !isAdmin)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ResponseMessage("User is unauthorized to delete this message board!")
                );
            }

            if (await service.DeleteAsync(id))
            {
                return Ok(new ResponseMessage("MessageBoard was deleted!"));
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseMessage("MessageBoard was not deleted!")
                );
            }
        }
    }
}
