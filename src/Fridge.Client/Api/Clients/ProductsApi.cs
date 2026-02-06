using Fridge.Client.Api.Dtos;

namespace Fridge.Client.Api.Clients;

public sealed record UpdateProductRequest(string Name, int? DefaultQuantity);

public class ProductsApi(HttpClient http) : ApiClientBase(http)
{

    public Task<List<ProductDto>?> GetProductsAsync(CancellationToken ct = default)
        => Http.GetFromJsonAsync<List<ProductDto>>($"/api/products", ct);

    public Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken ct = default)
        => GetOrNullIfNotFoundAsync<ProductDto>($"/api/products/{id}", ct);

    public Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken ct)
        => PostJsonAsync<ProductDto>($"/api/products/{id}", request, ct);

}