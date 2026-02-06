using Fridge.Client.Api.Dtos;

namespace Fridge.Client.Api.Clients;

public class FridgesApi(HttpClient http) : ApiClientBase(http)
{
    public Task<List<FridgeDto>?> GetFridgesAsync(CancellationToken ct = default)
        => Http.GetFromJsonAsync<List<FridgeDto>>("/api/fridges", ct);

    public Task<FridgeDto?> GetFridgeByIdAsync(Guid id, CancellationToken ct = default)
        => GetOrNullIfNotFoundAsync<FridgeDto>($"/api/fridges/{id}", ct);

    public Task<List<FridgeProductDto>?> GetFridgeProductsAsync(Guid fridgeId, CancellationToken ct = default)
        => Http.GetFromJsonAsync<List<FridgeProductDto>>($"/api/fridges/{fridgeId}/products", ct);
}

