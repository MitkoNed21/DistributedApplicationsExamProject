using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WebSite.Models;
using WebSite.Models.Message;
using WebSite.Models.MessageBoard;

namespace WebSite.Controllers
{
    public class MessagesController : ControllerBase
    {
        // GET: MessageController
        public async Task<IActionResult> Index(int? messageBoardId = null, IndexMessageViewModel? model = null)
        {
            UpdateHttpContextUser();
            if (!HttpContext.User.Claims.Any()) return Redirect("/users/login");
            var isAdmin = bool.Parse(HttpContext.User.FindFirst("isadmin")!.Value);

            if (messageBoardId is null && !isAdmin) return StatusCode(StatusCodes.Status403Forbidden);

            var messagesUri = "messages";

            if (messageBoardId is not null) messagesUri = $"messageBoards/{messageBoardId}/messages";

            if (model?.Filter is not null && !String.IsNullOrWhiteSpace(model?.Filter.Content))
            {
                messagesUri += "/filtered";
            }

            var response = await PerformRequest(HttpMethod.Get, messagesUri, model?.Filter);

            if (!response.IsSuccessStatusCode!.Value)
            {
                if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    return Redirect("/users/login");
                }

                return StatusCode(
                    response.StatusCode!.Value,
                    response.HttpResponseMessage
                );
            }

            using var stream = new MemoryStream();
            var messageBoardDataBytes = Encoding.UTF8.GetBytes(((JsonElement)response.Data!).GetRawText());

            await stream.WriteAsync(messageBoardDataBytes);
            stream.Seek(0, SeekOrigin.Begin);

            var messages = (await JsonSerializer.DeserializeAsync<List<MessageViewModel>>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            }))!;

            foreach (var item in messages)
            {
                item.CreatedByUsername = await GetUserNameForUserWithId(item.CreatedById);
                item.MessageBoardName = await GetMessageBoardNameForUserWithId(item.MessageBoardId);
            }

            ViewData["mbId"] = messageBoardId;

            model = new();
            if (messages is not null)
            {
                model.Messages = messages;
                model.Filter = new();
            }

            return View(model);
        }

        // GET: MessageController/Details/5
        public async Task<IActionResult> Details(int? messageBoardId, int id)
        {
            var response = await PerformRequest(HttpMethod.Get, $"messages/{id}", null);

            if (!response.IsSuccessStatusCode!.Value)
            {
                if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    return Redirect("/users/login");
                }

                return StatusCode(
                    response.StatusCode!.Value,
                    response.HttpResponseMessage
                );
            }

            using var stream = new MemoryStream();
            var userDataBytes = Encoding.UTF8.GetBytes(((JsonElement)response.Data!).GetRawText());

            await stream.WriteAsync(userDataBytes);
            stream.Seek(0, SeekOrigin.Begin);

            var model = (await JsonSerializer.DeserializeAsync<MessageViewModel>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            }))!;

            model.CreatedByUsername = await GetUserNameForUserWithId(model.CreatedById);
            model.MessageBoardName = await GetMessageBoardNameForUserWithId(model.MessageBoardId);

            ViewData["mbId"] = messageBoardId;

            return View(model);
        }

        // GET: MessageController/Create
        public async Task<IActionResult> Create(int? messageBoardId = null)
        {
            UpdateHttpContextUser();
            if (!HttpContext.User.Claims.Any()) return Redirect("/users/login");

            var model = new CreateEditMessageViewModel();

            var response = await PerformRequest(HttpMethod.Get, "messageBoards", null);

            if (!response.IsSuccessStatusCode!.Value)
            {
                if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    return Redirect("/users/login");
                }

                return StatusCode(
                    response.StatusCode!.Value,
                    response.HttpResponseMessage
                );
            }

            using var stream = new MemoryStream();
            var messageBoardDataBytes = Encoding.UTF8.GetBytes(((JsonElement)response.Data!).GetRawText());

            await stream.WriteAsync(messageBoardDataBytes);
            stream.Seek(0, SeekOrigin.Begin);

            var messageBoards = (await JsonSerializer.DeserializeAsync<IEnumerable<MessageBoardViewModel>>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            }))!;

            model.MessageBoards = messageBoards.Where(mb => mb.IsOpen).ToList();

            if (messageBoardId is not null) model.MessageBoardId = messageBoardId.Value;

            ViewData["mbId"] = messageBoardId;

            return View(model);
        }

        // POST: MessageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEditMessageViewModel model)
        {
            var response = await PerformRequest(HttpMethod.Post, $"messages", model);

            if (!response.IsSuccessStatusCode!.Value)
            {
                if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    return Redirect("/users/login");
                }

                return StatusCode(
                    response.StatusCode!.Value,
                    response.HttpResponseMessage
                );
            }

            try
            {
                return RedirectToAction(nameof(Index), new { messageBoardId = model.MessageBoardId });
            }
            catch
            {
                return View();
            }
        }

        // GET: MessageController/Edit/5
        public async Task<IActionResult> Edit(int? messageBoardId, int id)
        {
            UpdateHttpContextUser();
            if (!HttpContext.User.Claims.Any()) return Redirect("/users/login");

            var response = await PerformRequest(HttpMethod.Get, $"messages/{id}", null);

            if (!response.IsSuccessStatusCode!.Value)
            {
                if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    return Redirect("/users/login");
                }

                return StatusCode(
                    response.StatusCode!.Value,
                    response.HttpResponseMessage
                );
            }

            using var stream = new MemoryStream();
            var userDataBytes = Encoding.UTF8.GetBytes(((JsonElement)response.Data!).GetRawText());

            await stream.WriteAsync(userDataBytes);
            stream.Seek(0, SeekOrigin.Begin);

            var model = (await JsonSerializer.DeserializeAsync<CreateEditMessageViewModel>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            }))!;

            response = await PerformRequest(HttpMethod.Get, "messageBoards", null);

            if (!response.IsSuccessStatusCode!.Value)
            {
                if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    return Redirect("/users/login");
                }

                return StatusCode(
                    response.StatusCode!.Value,
                    response.HttpResponseMessage
                );
            }

            using var messageBoardDataStream = new MemoryStream();
            var messageBoardDataBytes = Encoding.UTF8.GetBytes(((JsonElement)response.Data!).GetRawText());

            await messageBoardDataStream.WriteAsync(messageBoardDataBytes);
            messageBoardDataStream.Seek(0, SeekOrigin.Begin);

            var messageBoards = (await JsonSerializer.DeserializeAsync<IEnumerable<MessageBoardViewModel>>(
                messageBoardDataStream, options: new()
                {
                    PropertyNameCaseInsensitive = true
                })
            )!;

            model.MessageBoards = messageBoards.Where(mb => mb.IsOpen).ToList();

            ViewData["mbId"] = messageBoardId;

            return View(model);
        }

        // POST: MessageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateEditMessageViewModel model)
        {
            var response = await PerformRequest(HttpMethod.Put, $"messages", model);

            if (!response.IsSuccessStatusCode!.Value)
            {
                if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    return Redirect("/users/login");
                }

                return StatusCode(
                    response.StatusCode!.Value,
                    response.HttpResponseMessage
                );
            }

            try
            {
                return RedirectToAction(nameof(Index), new { messageBoardId = model.MessageBoardId });
            }
            catch
            {
                return View();
            }
        }

        // GET: MessageController/Delete/5
        public async Task<IActionResult> Delete(int? messageBoardId, int id)
        {
            var response = await PerformRequest(HttpMethod.Get, $"messages/{id}", null);

            if (!response.IsSuccessStatusCode!.Value)
            {
                if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    return Redirect("/users/login");
                }

                return StatusCode(
                    response.StatusCode!.Value,
                    response.HttpResponseMessage
                );
            }

            using var stream = new MemoryStream();
            var userDataBytes = Encoding.UTF8.GetBytes(((JsonElement)response.Data!).GetRawText());

            await stream.WriteAsync(userDataBytes);
            stream.Seek(0, SeekOrigin.Begin);

            var model = (await JsonSerializer.DeserializeAsync<MessageViewModel>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            }))!;

            model.CreatedByUsername = await GetUserNameForUserWithId(model.CreatedById);
            model.MessageBoardName = await GetUserNameForUserWithId(model.MessageBoardId);

            ViewData["mbId"] = messageBoardId;

            return View(model);
        }

        // POST: MessageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, MessageViewModel model)
        {
            var response = await PerformRequest(HttpMethod.Delete, $"messages/{id}", model);

            if (!response.IsSuccessStatusCode!.Value)
            {
                if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    return Redirect("/users/login");
                }

                // TODO: Forbidden

                return StatusCode(
                    response.StatusCode!.Value,
                    response.HttpResponseMessage
                );
            }

            try
            {
                return RedirectToAction(nameof(Index), new { messageBoardId = model.MessageBoardId });
            }
            catch
            {
                return View();
            }
        }

        [NonAction]
        private async Task<string> GetMessageBoardNameForUserWithId(int id)
        {
            var response = await PerformRequest(HttpMethod.Get, $"messageBoards/{id}", null);

            if (!response.IsSuccessStatusCode!.Value)
            {
                return null;
            }

            return ((JsonElement)response.Data!).GetProperty("name").ToString();
        }
    }
}
