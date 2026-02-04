using Fridge.Client.Api;
using Microsoft.AspNetCore.Mvc;

namespace Fridge.Client.Controllers;

public class FridgesController(FridgeApiClient api) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var fridges = await api.GetFridgesAsync(ct) ?? [];
        return View(fridges);
    }

    public async Task<IActionResult> Products(Guid id, CancellationToken ct)
    {
        var products = await api.GetFridgeProductsAsync(id, ct) ?? [];
        ViewBag.FridgeId = id;
        return View(products);
    }
}