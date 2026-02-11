using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fridge.Client.Models.ViewModels.Fridges;

public sealed class CreateFridgeVm
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? OwnerName { get; set; }

    [Required]
    public Guid ModelId { get; set; }

    public List<SelectListItem>? Models { get; set; } = [];
    public List<SelectListItem>? Products { get; set; } = [];

    // initial list
    public List<InitialProductVm>? InitialProducts { get; set; } = [];

    public Guid? ProductIdToAdd { get; set; }
    public int QuantityToAdd { get; set; } = 1;
}

public sealed class InitialProductVm
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}