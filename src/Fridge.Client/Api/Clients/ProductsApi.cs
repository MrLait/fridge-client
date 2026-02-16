using Fridge.Client.Api.Dtos;

namespace Fridge.Client.Api.Clients;

public sealed record UpdateProductRequest(string Name, int? DefaultQuantity);

public class ProductsApi(HttpClient http) : ApiClientBase(http)
{
    public Task<List<ProductListDto>> GetAllAsync(CancellationToken ct = default)
        => GetRequiredAsync<List<ProductListDto>>($"/api/products", ct);

    public Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => GetOrNullIfNotFoundAsync<ProductDto>($"/api/products/{id}", ct);

    public Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct)
        => PutJsonAsync<ProductDto>($"/api/products/{id}", request, ct);
}