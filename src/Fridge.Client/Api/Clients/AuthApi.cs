
namespace Fridge.Client.Api.Clients;

public sealed class AuthApi(HttpClient http) : ApiClientBase(http)
{
    public sealed record LoginRequest(string Username, string Password);
    public sealed record LoginResponse(string AccessToken);
    public sealed record MeResponse(string Username, string Role, Dictionary<string, string[]> Claims);

    public async Task<string> LoginAsync(string username, string password, CancellationToken ct)
    {
        var body = await PostJsonAsync<LoginResponse>("/api/auth/login", new LoginRequest(username, password), ct);

        if (string.IsNullOrWhiteSpace(body.AccessToken))
            throw new InvalidOperationException("Empty accessToken.");

        return body.AccessToken;
    }

    public Task<MeResponse> MeAsync(CancellationToken ct = default)
    => GetRequiredAsync<MeResponse>("/api/auth/me", ct);
}