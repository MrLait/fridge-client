using Fridge.Client.Api.Dtos;

namespace Fridge.Client.Api.Clients;

public class FridgesApi(HttpClient http) : ApiClientBase(http)
{
    public Task<List<FridgeDto>> GetAllAsync(CancellationToken ct = default)
        => GetRequiredAsync<List<FridgeDto>>("/api/fridges", ct);

    public Task<FridgeDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => GetOrNullIfNotFoundAsync<FridgeDto>($"/api/fridges/{id}", ct);

    public Task<List<FridgeProductDto>> GetProductsAsync(Guid fridgeId, CancellationToken ct = default)
        => GetRequiredAsync<List<FridgeProductDto>>($"/api/fridges/{fridgeId}/products", ct);

    public async Task<Guid> CreateAsync(CreateFridgeRequest request, CancellationToken ct = default)
        => (await PostJsonAsync<CreateFridgeResponse>($"/api/fridges", request, ct)).Id;

    public Task UpdateAsync(Guid id, UpdateFridgeRequest request, CancellationToken ct = default)
        => PutAsync($"/api/fridges/{id}", request, ct);

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => DeleteAsync($"/api/fridges/{id}", ct);

    public async Task<Guid> AddProductAsync(Guid id, AddProductToFridgeRequest body, CancellationToken ct)
        => (await PostJsonAsync<AddProductResponse>($"api/fridges/{id}/products", body, ct)).Id;

    public sealed record CreateFridgeRequest(
        string Name,
        string? OwnerName,
        Guid ModelId,
        IEnumerable<FridgeProductItem>? InitialProducts
    );

    public sealed record FridgeProductItem(Guid ProductId, int Quantity);
    public sealed record UpdateFridgeRequest(string Name, string? OwnerName, Guid ModelId);
    public sealed record AddProductToFridgeRequest(Guid ProductId, int Quantity);
    public sealed record CreateFridgeResponse(Guid Id);
    public sealed record AddProductResponse(Guid Id);
}

