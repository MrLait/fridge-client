using System.ComponentModel.DataAnnotations;
using Fridge.Client.Api.Dtos;

namespace Fridge.Client.Models.ViewModels.Products;

public sealed class EditProductVm
{
    [Required]
    public Guid Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int? DefaultQuantity { get; set; }


    public List<ProductImageDto> Images { get; set; } = new();

}