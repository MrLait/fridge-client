
using System.Net;

namespace Fridge.Client.Api;

public class ApiClientBase(HttpClient http)
{
    protected HttpClient Http => http;
    protected async Task<T> ReadJsonRequiredAsync<T>(HttpResponseMessage resp, CancellationToken ct)
        => await resp.Content.ReadFromJsonAsync<T>(cancellationToken: ct)
           ?? throw new InvalidOperationException("Empty response body.");

    protected async Task<T?> GetOrNullIfNotFoundAsync<T>(string url, CancellationToken ct)
    {
        using var resp = await Http.GetAsync(url, ct);

        if (resp.StatusCode == HttpStatusCode.NotFound)
            return default;

        resp.EnsureSuccessStatusCode();
        return await ReadJsonRequiredAsync<T>(resp, ct);
    }

    protected async Task<T> PostJsonAsync<T>(string url, object body, CancellationToken ct)
    {
        using var resp = await Http.PostAsJsonAsync(url, body, ct);
        resp.EnsureSuccessStatusCode();
        return await ReadJsonRequiredAsync<T>(resp, ct);
    }
}