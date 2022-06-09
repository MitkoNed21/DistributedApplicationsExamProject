using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WebSite.Models;
using WebSite.Models.MessageBoard;

namespace WebSite.Controllers
{
    public class MessageBoardsController : ControllerBase
    {
        // GET: MessageBoardsController
        public async Task<IActionResult> Index(IndexMessageBoardViewModel? model = null)
        {
            var url = "messageBoards";
            if (model?.Filter is not null && !String.IsNullOrWhiteSpace(model?.Filter.Name))
            {
                url += "/filtered";
            }

            var response = await PerformRequest(HttpMethod.Get, url, model?.Filter);

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

            var messageBoards = (await JsonSerializer.DeserializeAsync<List<MessageBoardViewModel>>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            }))!;

            foreach (var item in messageBoards)
            {
                item.CreatedByUsername = await GetUserNameForUserWithId(item.CreatedById);
                item.UpdatedByUsername = await GetUserNameForUserWithId(item.UpdatedById);

                item.UpdatedOn = item.UpdatedOn.ToLocalTime();
            }

            model = new IndexMessageBoardViewModel();
            if (messageBoards is not null)
            {
                model.MessageBoards = messageBoards;
                model.Filter = new();
            }

            return View(model);
        }

        // GET: MessageBoardsController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var response = await PerformRequest(HttpMethod.Get, $"messageBoards/{id}", null);

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
            var dataBytes = Encoding.UTF8.GetBytes(((JsonElement)response.Data!).GetRawText());

            await stream.WriteAsync(dataBytes);
            stream.Seek(0, SeekOrigin.Begin);

            var model = (await JsonSerializer.DeserializeAsync<MessageBoardViewModel>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            }))!;

            model.CreatedByUsername = await GetUserNameForUserWithId(model.CreatedById);
            model.UpdatedByUsername = await GetUserNameForUserWithId(model.UpdatedById);

            model.UpdatedOn = model.UpdatedOn.ToLocalTime();

            response = await PerformRequest(HttpMethod.Get, $"messageBoards/{id}/messages", null);

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

            using var messagesStream = new MemoryStream();
            dataBytes = Encoding.UTF8.GetBytes(((JsonElement)response.Data!).GetRawText());

            await messagesStream.WriteAsync(dataBytes);
            messagesStream.Seek(0, SeekOrigin.Begin);

            var messages = (await JsonSerializer.DeserializeAsync<IEnumerable<Models.Message.MessageViewModel>>(
                messagesStream, options: new()
                {
                    PropertyNameCaseInsensitive = true
                }
            ))!;

            foreach (var message in messages)
            {
                message.CreatedByUsername = await GetUserNameForUserWithId(message.CreatedById);
            }

            model.Messages = messages.ToList();

            return View(model);
        }

        // GET: MessageBoardsController/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: MessageBoardsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MessageBoardViewModel model)
        {
            var response = await PerformRequest(HttpMethod.Post, $"messageBoards", model);

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

            //using var stream = new MemoryStream();
            //var userDataBytes = Encoding.UTF8.GetBytes(((JsonElement)response.Data!).GetRawText());

            //await stream.WriteAsync(userDataBytes);
            //stream.Seek(0, SeekOrigin.Begin);

            //TODO: if response has some answer display to user if significant?

            //var model = (await JsonSerializer.DeserializeAsync<MessageBoardViewModel>(stream, options: new()
            //{
            //    PropertyNameCaseInsensitive = true
            //}))!;

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MessageBoardsController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await PerformRequest(HttpMethod.Get, $"messageBoards/{id}", null);

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

            var model = (await JsonSerializer.DeserializeAsync<MessageBoardViewModel>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            }))!;

            return View(model);
        }

        // POST: MessageBoardsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MessageBoardViewModel model)
        {
            if (id != model.Id) RedirectToAction(nameof(Index));

            var response = await PerformRequest(HttpMethod.Put, $"messageBoards", model);

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

            //using var stream = new MemoryStream();
            //var userDataBytes = Encoding.UTF8.GetBytes(((JsonElement)response.Data!).GetRawText());

            //await stream.WriteAsync(userDataBytes);
            //stream.Seek(0, SeekOrigin.Begin);

            //TODO: if response has some answer display to user if significant?

            //var model = (await JsonSerializer.DeserializeAsync<MessageBoardViewModel>(stream, options: new()
            //{
            //    PropertyNameCaseInsensitive = true
            //}))!;

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MessageBoardsController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var response = await PerformRequest(HttpMethod.Get, $"messageBoards/{id}", null);

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

            var model = (await JsonSerializer.DeserializeAsync<MessageBoardViewModel>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            }))!;

            model.CreatedByUsername = await GetUserNameForUserWithId(model.CreatedById);
            model.UpdatedByUsername = await GetUserNameForUserWithId(model.UpdatedById);

            model.UpdatedOn = model.UpdatedOn.ToLocalTime();

            return View(model);
        }

        // POST: MessageBoardsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, MessageBoardViewModel model)
        {
            var response = await PerformRequest(HttpMethod.Delete, $"messageBoards/{id}", model);

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
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
