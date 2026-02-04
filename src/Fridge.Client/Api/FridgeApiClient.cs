using Fridge.Client.Api.Dtos;

namespace Fridge.Client.Api;

public sealed class FridgeApiClient(HttpClient http)
{
    public async Task<List<FridgeDto>?> GetFridgesAsync(CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<FridgeDto>>("/api/fridges", ct);

    public async Task<List<FridgeProductDto>?> GetFridgeProductsAsync(Guid fridgeId, CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<FridgeProductDto>>($"/api/fridges/{fridgeId}/products", ct);

    public async Task<int> RestockZeroQuantityToDefaultAsync(CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/maintenance/restock-zero", new { }, ct);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<RestockResponse>(cancellationToken: ct);

        return body?.Updated ?? 0;
    }

    private sealed record RestockResponse(int Updated);
}
