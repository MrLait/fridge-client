using System.ComponentModel.DataAnnotations;
using Fridge.Client.Api.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fridge.Client.Models;

public sealed class FridgeProductsVm
{
    public Guid FridgeId { get; set; }
    public string? FridgeName { get; set; }
    public List<FridgeProductDto> Items { get; set; } = [];

    // add form
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    public List<SelectListItem> Products { get; set; } = [];
}