
using System.Net;
using Microsoft.AspNetCore.Mvc;

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

        await EnsureSuccessAsync(resp, ct);
        return await ReadJsonRequiredAsync<T>(resp, ct);
    }

    protected async Task<T> GetRequiredAsync<T>(string url, CancellationToken ct)
    {
        using var resp = await Http.GetAsync(url, ct);
        await EnsureSuccessAsync(resp, ct);
        return await ReadJsonRequiredAsync<T>(resp, ct);
    }

    protected async Task<T> PostJsonAsync<T>(string url, object? body, CancellationToken ct)
    {
        using var resp = await Http.PostAsJsonAsync(url, body, ct);
        await EnsureSuccessAsync(resp, ct);
        return await ReadJsonRequiredAsync<T>(resp, ct);
    }

    protected async Task PostAsync(string url, CancellationToken ct)
    {
        using var resp = await Http.PostAsync(url, content: null, ct);
        await EnsureSuccessAsync(resp, ct);
    }

    protected async Task<T> PostMultipartAsync<T>(string url, MultipartFormDataContent form, CancellationToken ct)
    {
        using var resp = await Http.PostAsync(url, form, ct);
        await EnsureSuccessAsync(resp, ct);
        return await ReadJsonRequiredAsync<T>(resp, ct);
    }

    protected async Task<T> PutJsonAsync<T>(string url, object body, CancellationToken ct)
    {
        using var resp = await Http.PutAsJsonAsync(url, body, ct);
        await EnsureSuccessAsync(resp, ct);
        return await ReadJsonRequiredAsync<T>(resp, ct);
    }
    protected async Task PutAsync(string url, object body, CancellationToken ct)
    {
        using var resp = await Http.PutAsJsonAsync(url, body, ct);
        await EnsureSuccessAsync(resp, ct);
    }

    protected async Task<bool> DeleteAsync(string url, CancellationToken ct)
    {
        using var resp = await Http.DeleteAsync(url, ct);

        if (resp.StatusCode == HttpStatusCode.NotFound)
            return false;

        await EnsureSuccessAsync(resp, ct);

        return true;
    }


    protected async Task EnsureSuccessAsync(HttpResponseMessage resp, CancellationToken ct)
    {
        if (resp.IsSuccessStatusCode)
            return;

        ProblemDetails? pd = null;

        try
        {
            pd = await resp.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: ct);
        }
        catch { /* ignore */ }

        var message = pd?.Title is not null
            ? $"{pd.Title}: {pd.Detail}"
            : $"Request failed: {(int)resp.StatusCode} {resp.ReasonPhrase}";

        throw new HttpRequestException(message, null, resp.StatusCode);
    }
}