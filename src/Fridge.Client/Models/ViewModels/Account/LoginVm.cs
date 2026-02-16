using System.ComponentModel.DataAnnotations;

namespace Fridge.Client.Models.ViewModels.Account;

public sealed class LoginVm
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}