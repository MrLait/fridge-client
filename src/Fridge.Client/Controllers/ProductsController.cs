using Fridge.Client.Api;
using Fridge.Client.Api.Clients;
using Fridge.Client.Models;
using Fridge.Client.Models.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Fridge.Client.Controllers;

public sealed class ProductsController(ProductsApi productsApi, ProductImagesApi productImagesApi) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var products = await productsApi.GetAllAsync(ct) ?? [];
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var product = await productsApi.GetByIdAsync(id, ct);

        if (product is null)
            return NotFound();

        var images = await productImagesApi.GetImagesAsync(product.Id, ct);

        return View(
            new EditProductVm
            {
                Id = product.Id,
                Name = product.Name,
                DefaultQuantity = product.DefaultQuantity,
                Images = images
            }
        );
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditProductVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var updated = await productsApi.UpdateAsync(vm.Id, new UpdateProductRequest(vm.Name, vm.DefaultQuantity), ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadImageAjax(Guid productId, IFormFile file, CancellationToken ct)
    {
        if (productId == Guid.Empty)
            return BadRequest("Invalid productId.");

        if (file is null || file.Length == 0)
            return BadRequest("File is empty.");

        if (file.ContentType is not ("image/jpeg" or "image/png"))
            return BadRequest("Only jpg/png allowed.");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("Max file size is 5MB.");

        await using var stream = file.OpenReadStream();
        await productImagesApi.UploadAsync(productId, stream, file.FileName, file.ContentType, ct);

        return await RenderImagesPartial(productId, ct);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImageAjax(Guid productId, Guid imageId, CancellationToken ct)
    {
        if (productId == Guid.Empty || imageId == Guid.Empty)
            return BadRequest("Invalid input.");

        await productImagesApi.DeleteAsync(productId, imageId, ct);

        return await RenderImagesPartial(productId, ct);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPrimaryImageAjax(Guid productId, Guid imageId, CancellationToken ct)
    {
        if (productId == Guid.Empty || imageId == Guid.Empty)
            return BadRequest("Invalid input.");

        await productImagesApi.SetPrimaryAsync(productId, imageId, ct);

        return await RenderImagesPartial(productId, ct);
    }

    private async Task<IActionResult> RenderImagesPartial(Guid productId, CancellationToken ct)
    {
        var product = await productsApi.GetByIdAsync(productId, ct);

        if (product is null)
            return NotFound();

        var images = await productImagesApi.GetImagesAsync(productId, ct);

        var vm = new EditProductVm
        {
            Id = product.Id,
            Name = product.Name,
            DefaultQuantity = product.DefaultQuantity,
            Images = images
        };

        return PartialView("_ProductImages", vm);
    }
}