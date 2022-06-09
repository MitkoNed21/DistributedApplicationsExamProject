using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace WebSite.Controllers
{
    public class ControllerBase : Controller
    {

        [NonAction]
        public async Task<string> GetUserNameForUserWithId(int id)
        {
            var response = await PerformRequest(HttpMethod.Get, $"users/{id}", null);

            if (!response.IsSuccessStatusCode!.Value)
            {
                return null;
            }

            return ((JsonElement)response.Data!).GetProperty("userName").ToString();
        }
        [NonAction]
        public async Task<ResponseMessage> PerformRequest(HttpMethod method, string url, object? content)
        {
            var request = new HttpRequestMessage(method, url);

            var token = HttpContext.Session.GetString("token");
            if (token is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var contentStream = new MemoryStream();

            await JsonSerializer.SerializeAsync(contentStream, content);
            var stringContent = Encoding.UTF8.GetString(contentStream.ToArray());

            request.Content = new StringContent(stringContent, Encoding.UTF8, MediaTypeNames.Application.Json);

            var httpClient = HttpContext.RequestServices.GetService<HttpClient>()!;
            var response = await httpClient.SendAsync(request);

            var responseData = await response.Content.ReadFromJsonAsync<ResponseMessage>() ?? new();
            responseData.HttpResponseMessage = response;

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                HttpContext.User = null;
            }
            else if (responseData.Token is not null)
            {
                HttpContext.Session.SetString("token", responseData.Token);

                UpdateHttpContextUser(responseData.Token);
            }

            return responseData;
        }

        [NonAction]
        public void UpdateHttpContextUser(string? token = null)
        {
            if (HttpContext is null) return;

            if (token is null) token = HttpContext.Session.GetString("token");
            if (token is null)
            {
                HttpContext.User = null;
                return;
            }

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                HttpContext.User = null;
                return;
            }

            var readToken = handler.ReadJwtToken(token);

            var currentTime = DateTime.UtcNow;
            if (readToken.ValidFrom > currentTime || readToken.ValidTo < currentTime)
            {
                HttpContext.User = null;
                return;
            }

            HttpContext.User = new ClaimsPrincipal();

            var identity = new ClaimsIdentity();
            identity.AddClaims(readToken.Claims.Where(c =>
                !c.Type.Contains("exp") &&
                !c.Type.Contains("iat") &&
                !c.Type.Contains("eam") &&
                !c.Type.Contains("nbf")
            ));

            HttpContext.User = new ClaimsPrincipal(identity);
        }
    }
}
