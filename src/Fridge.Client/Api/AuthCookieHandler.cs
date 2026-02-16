using System.Net.Http.Headers;

namespace Fridge.Client.Api;

public sealed class AuthCookieHandler(IHttpContextAccessor accessor)
    : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = accessor.HttpContext?.Request.Cookies["access_token"];

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, ct);
    }
}