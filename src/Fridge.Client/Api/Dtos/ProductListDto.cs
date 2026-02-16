namespace Fridge.Client.Api.Dtos;

public sealed record ProductListDto(
    Guid Id,
    string Name,
    int? DefaultQuantity,
    string? PrimaryImageUrl
);