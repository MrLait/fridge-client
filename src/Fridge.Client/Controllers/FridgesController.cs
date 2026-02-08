using Fridge.Client.Api.Clients;
using Microsoft.AspNetCore.Mvc;

namespace Fridge.Client.Controllers;

public class FridgesController(FridgesApi api) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var fridges = await api.GetAllAsync(ct) ?? [];
        return View(fridges);
    }

    public async Task<IActionResult> Products(Guid id, CancellationToken ct)
    {
        var products = await api.GetProductsAsync(id, ct) ?? [];
        ViewBag.FridgeId = id;
        return View(products);
    }
}