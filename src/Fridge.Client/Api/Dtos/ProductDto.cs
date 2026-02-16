namespace Fridge.Client.Api.Dtos;

public sealed record ProductDto(
    Guid Id,
    string Name,
    int? DefaultQuantity
);
