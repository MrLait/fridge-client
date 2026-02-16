using System.Net.Http.Headers;
using Fridge.Client.Api.Dtos;

namespace Fridge.Client.Api.Clients;

public sealed class ProductImagesApi(HttpClient http) : ApiClientBase(http)
{
    private sealed record UploadResponse(Guid ImageId);

    public Task<List<ProductImageDto>> GetImagesAsync(Guid productId, CancellationToken ct = default)
        => GetRequiredAsync<List<ProductImageDto>>($"api/products/{productId}/images", ct);

    public async Task<Guid> UploadAsync(Guid productId, Stream fileStream, string fileName, string contentType, CancellationToken ct = default)
    {
        using var form = new MultipartFormDataContent();

        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        form.Add(fileContent, "File", fileName);

        var body = await PostMultipartAsync<UploadResponse>($"/api/products/{productId}/images", form, ct);

        return body.ImageId;
    }

    public Task SetPrimaryAsync(Guid productId, Guid imageId, CancellationToken ct = default)
        => PutAsync($"/api/products/{productId}/images/{imageId}/primary", new { }, ct);
    public Task<List<ProductImageDto>> GetProductImagesAsync(Guid productId, CancellationToken ct = default)
        => GetRequiredAsync<List<ProductImageDto>>($"api/products/{productId}/images", ct);
    public Task<bool> DeleteAsync(Guid productId, Guid imageId, CancellationToken ct = default)
        => DeleteAsync($"/api/products/{productId}/images/{imageId}", ct);
}