using Fridge.Client.Api.Dtos;

namespace Fridge.Client.Api.Clients;

public class FridgeModelsApi(HttpClient http) : ApiClientBase(http)
{
    public Task<List<FridgeModelDto>> GetAllAsync(CancellationToken ct = default)
        => GetRequiredAsync<List<FridgeModelDto>>("/api/fridge-models", ct);
}