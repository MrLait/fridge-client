
namespace Fridge.Client.Api.Clients;

public sealed class MaintenanceApi(HttpClient http) : ApiClientBase(http)
{
    private sealed record RestockResponse(int Updated);

    public async Task<int> RestockZeroQuantityToDefaultAsync(CancellationToken ct = default)
    {
        var resp = await PostJsonAsync<RestockResponse>("/api/maintenance/restock-zero", new { }, ct);

        return resp.Updated;
    }
}
