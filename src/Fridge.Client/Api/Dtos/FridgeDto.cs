namespace Fridge.Client.Api.Dtos;

public sealed record FridgeDto
(
    Guid Id,
    string Name,
    string? OwnerName,
    Guid ModelId,
    string ModelName,
    int? ModelYear
);