using Microsoft.AspNetCore.Mvc;
using Fridge.Client.Api;
using Fridge.Client.Api.Clients;

namespace Fridge.Client.Controllers;

public class HomeController(MaintenanceApi api) : Controller
{
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Restock(CancellationToken ct)
    {
        var updated = await api.RestockZeroQuantityToDefaultAsync(ct);
        TempData["RestockResult"] = $"Restock completed. Updated: {updated}";
        return RedirectToAction(nameof(Index));
    }
}
