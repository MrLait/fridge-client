using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fridge.Client.Models;

public sealed class FridgeFormVm
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? OwnerName { get; set; }

    [Required]
    public Guid ModelId { get; set; }

    public List<SelectListItem>? Models { get; set; } = [];
}