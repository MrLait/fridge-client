using Fridge.Client.Api.Clients;
using Fridge.Client.Models.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace Fridge.Client.Controllers;

public sealed class AccountController(AuthApi authApi) : Controller
{
    [HttpGet]
    public IActionResult Login()
        => View(new LoginVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var token = await authApi.LoginAsync(vm.Username, vm.Password, ct);

            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });

            return RedirectToAction("Index", "Home");
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Invalid username/password.");
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        return RedirectToAction("Index", "Home");
    }
}
