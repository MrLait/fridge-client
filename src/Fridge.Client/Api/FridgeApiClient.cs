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

    public async Task<List<ProductDto>?> GetProductsAsync(CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<ProductDto>>($"/api/products", ct);

    public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await http.GetAsync($"/api/products/{id}", ct);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var product = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: ct);
        return product ?? throw new InvalidOperationException("Empty response body.");
    }

    public async Task<ProductDto> UpdateProduct(Guid id, UpdateProductRequest request, CancellationToken ct)
    {
        var response = await http.PutAsJsonAsync($"/api/products/{id}", request, ct);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: ct);

        return body!;
    }

    private sealed record RestockResponse(int Updated);
    public sealed record UpdateProductRequest(string Name, int? DefaultQuantity);
}
