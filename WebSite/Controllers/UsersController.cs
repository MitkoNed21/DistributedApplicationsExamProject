using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;
using WebSite.Models;
using System;
using System.Text.Json;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebSite.Models.User;

namespace WebSite.Controllers
{
    public class UsersController : ControllerBase
    {
        private readonly HttpClient httpClient;

        private readonly string BEARER_START = "Bearer ";

        public UsersController(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }


        public async Task<IActionResult> Login()
        {
            UpdateHttpContextUser();
            if (HttpContext.User.Claims.Any()) return Redirect("/");
            
            return View(new UserAuthViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserAuthViewModel model)
        {
            UpdateHttpContextUser();
            if (HttpContext.User.Claims.Any()) return Redirect("/");

            var response = await PerformRequest(HttpMethod.Post, $"users/login", model);

            if (response.HttpResponseMessage!.IsSuccessStatusCode) return Redirect("/");

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.User = null;
            HttpContext.Session.Remove("token");

            return Redirect("/");
        }

        public async Task<IActionResult> Register()
        {
            UpdateHttpContextUser();
            if (HttpContext.User.Claims.Any()) return Redirect("/");

            return View(new UserRegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            UpdateHttpContextUser();
            if (HttpContext.User.Claims.Any()) return Redirect("/");

            var response = await PerformRequest(HttpMethod.Post, $"users/register", model);

            if (!response.HttpResponseMessage!.IsSuccessStatusCode)
            {
                // error from response to model
                return View();
            }

            response = await PerformRequest(HttpMethod.Post, $"users/login", model);

            if (!response.HttpResponseMessage!.IsSuccessStatusCode)
            {
                // ???
                return RedirectToAction(nameof(Login));
            }

            return Redirect("/");
        }

        // GET: UsersController
        public async Task<IActionResult> Index(IndexUserViewModel? model = null)
        {
            UpdateHttpContextUser();
            if (!HttpContext.User.Claims.Any()) return Redirect("/users/login");

            var url = "users";
            if (model?.Filter is not null && !String.IsNullOrWhiteSpace(model?.Filter.UserName))
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
            var userDataBytes = Encoding.UTF8.GetBytes(((JsonElement)response.Data!).GetRawText());

            await stream.WriteAsync(userDataBytes);
            stream.Seek(0, SeekOrigin.Begin);

            var users = await JsonSerializer.DeserializeAsync<List<UserViewModel>>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            });

            model = new IndexUserViewModel();
            if (users is not null)
            {
                model.Users = users;
                model.Filter = new();
            }

            return View(model);
        }

        // GET: UsersController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var response = await PerformRequest(HttpMethod.Get, $"users/{id}", null);

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

            var model = await JsonSerializer.DeserializeAsync<UserViewModel>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            });

            return View(model);
        }

        // GET: UsersController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            UpdateHttpContextUser();
            if (!HttpContext.User.Claims.Any()) return Redirect("/users/login");

            var response = await PerformRequest(HttpMethod.Get, $"users/{id}", null);

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

            var model = (await JsonSerializer.DeserializeAsync<UserViewModel>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            }))!;

            return View(model);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserViewModel model)
        {
            var response = await PerformRequest(HttpMethod.Put, $"users", model);

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
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var response = await PerformRequest(HttpMethod.Get, $"users/{id}", null);

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

            var model = await JsonSerializer.DeserializeAsync<UserViewModel>(stream, options: new()
            {
                PropertyNameCaseInsensitive = true
            });

            return View(model); 
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, UserViewModel model)
        {
            UpdateHttpContextUser();
            var currentUserId = int.Parse(HttpContext.User.FindFirst("id")!.Value);

            var response = await PerformRequest(HttpMethod.Delete, $"users/{id}", model);

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

            if (currentUserId == id)
            {
                HttpContext.User = null;
                HttpContext.Session.Remove("token");
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

        [HttpGet]
        public async Task<IActionResult> Promote(int id)
        {
            var response = await PerformRequest(HttpMethod.Put, $"users/promote/{id}", null);

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

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Demote(int id)
        {
            var response = await PerformRequest(HttpMethod.Put, $"users/demote/{id}", null);

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

            return RedirectToAction(nameof(Index));
        }
    }
}