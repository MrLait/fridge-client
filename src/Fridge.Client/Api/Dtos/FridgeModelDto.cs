namespace Fridge.Client.Api.Dtos;

public sealed record FridgeModelDto
(
        Guid Id,
        string Name,
        int? Year
);