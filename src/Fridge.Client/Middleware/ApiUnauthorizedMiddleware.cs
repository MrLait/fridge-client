using System.Net;

namespace Fridge.Client.Middleware;

public sealed class ApiUnauthorizedMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (context.Request.Headers.TryGetValue("X-Requested-With", out var v) && v == "XMLHttpRequest")
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            context.Response.Redirect("/Account/Login");
        }
    }
}