namespace Fridge.Client.Api.Clients;

public sealed class FridgeProductApi(HttpClient http) : ApiClientBase(http)
{
    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken ct = default)
        => DeleteAsync($"api/fridge-products/{id}", ct);
}