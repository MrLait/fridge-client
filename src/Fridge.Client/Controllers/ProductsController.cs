using Fridge.Client.Api;
using Fridge.Client.Api.Clients;
using Fridge.Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fridge.Client.Controllers;

public sealed class ProductsController(ProductsApi api) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var products = await api.GetProductsAsync(ct) ?? [];
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var product = await api.GetProductByIdAsync(id, ct);

        if (product is null)
            return NotFound();

        return View(
            new EditProductVm
            {
                Id = product.Id,
                Name = product.Name,
                DefaultQuantity = product.DefaultQuantity
            }
        );
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditProductVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var updated = await api.UpdateProductAsync(vm.Id, new UpdateProductRequest(vm.Name, vm.DefaultQuantity), ct);
        return RedirectToAction(nameof(Index));
    }
}