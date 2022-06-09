using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using ApplicationServices.Implementations;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Utilities;

namespace WebApi.ActionFilters
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        string? newToken;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            const string BEARER_START = "Bearer ";

            var authService = context.HttpContext.RequestServices.GetService<AuthenticationManagementService>()!;
            var token = context.HttpContext.Request.Headers.Authorization
                .FirstOrDefault(x => x.Contains(BEARER_START))?[BEARER_START.Length..];

            string? newToken;
            if (!String.IsNullOrWhiteSpace(token))
                newToken = await authService.RefreshTokenAsync(token);
            else
            {
                context.Result = new UnauthorizedObjectResult(
                    new ResponseMessage("Authorization token required!")
                );
                return;
            }

            if (newToken is null)
            {
                context.Result = new UnauthorizedObjectResult(
                    new ResponseMessage("Invalid authorization token!")
                );
                return;
            }

            //new Microsoft.AspNetCore.Identity.IdentityUserToken<int>();

            var authenticatedUser = (await authService.GetAuthenticatedUserAsync(token))!;

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("id", authenticatedUser.Id.ToString()));
            claimsIdentity.AddClaim(new Claim("firstName", authenticatedUser.FirstName ?? ""));
            claimsIdentity.AddClaim(new Claim("lastName", authenticatedUser.LastName ?? ""));
            claimsIdentity.AddClaim(new Claim("userName", authenticatedUser.UserName.ToString()));
            claimsIdentity.AddClaim(new Claim("isAdmin", authenticatedUser.IsAdmin.ToString()));

            context.HttpContext.User = new ClaimsPrincipal(claimsIdentity);

            this.newToken = newToken;
            await next();
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (newToken is not null)
            {
                if (context.Result is ObjectResult or)
                {
                    or.Value = new TokenDataResponse<object?>(or.Value, newToken);
                }
                else if (context.Result is OkResult)
                {
                    context.Result = new OkObjectResult(new TokenResponseMessage(null, newToken));
                }
            }

            await next();

            newToken = null; // TODO: test if this is kept between different classes
        }
    }
}
