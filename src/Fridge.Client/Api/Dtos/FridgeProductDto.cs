namespace Fridge.Client.Api.Dtos;

public sealed record FridgeProductDto
(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    int? ProductDefaultQuantity
);