using Microsoft.AspNetCore.Mvc;
using Fridge.Client.Api;

namespace Fridge.Client.Controllers;

public class HomeController(FridgeApiClient api) : Controller
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
