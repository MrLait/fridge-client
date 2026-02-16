using Fridge.Client.Api.Clients;

namespace Fridge.Client.Services;

public sealed class CurrentUserService(AuthApi authApi)
{
    private AuthApi.MeResponse? _cached;

    public async Task<AuthApi.MeResponse?> GetMeAsync(CancellationToken ct = default)
    {
        if (_cached is not null) return _cached;

        try
        {
            _cached = await authApi.MeAsync(ct);
            return _cached;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return null;
        }
    }
}