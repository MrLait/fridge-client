namespace Fridge.Client.Models;

public sealed class EditProductVm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int? DefaultQuantity { get; set; }

}