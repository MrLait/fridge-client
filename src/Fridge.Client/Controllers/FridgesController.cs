using Fridge.Client.Api.Clients;
using Fridge.Client.Models;
using Fridge.Client.Models.ViewModels.Fridges;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Fridge.Client.Api.Clients.FridgesApi;

namespace Fridge.Client.Controllers;

public class FridgesController(
    FridgesApi fridgesApi, FridgeModelsApi fridgeModelsApi, ProductsApi productsApi, FridgeProductApi fridgeProductApi) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var fridges = await fridgesApi.GetAllAsync(ct) ?? [];

        return View(fridges);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new CreateFridgeVm();
        await PopulateCreateVmList(vm, ct);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateFridgeVm vm, CancellationToken ct)
    {
        await PopulateCreateVmList(vm, ct);

        if (!ModelState.IsValid)
            return View(vm);

        var initialProducts = (vm.InitialProducts ?? [])
            .Select(x => new FridgeProductItem(x.ProductId, x.Quantity))
            .ToList();

        var id = await fridgesApi.CreateAsync(new CreateFridgeRequest(vm.Name, vm.OwnerName, vm.ModelId, initialProducts), ct);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddInitialProduct(CreateFridgeVm vm, CancellationToken ct)
    {
        await PopulateCreateVmList(vm, ct);

        vm.InitialProducts ??= [];

        if (vm.ProductIdToAdd == Guid.Empty)
            ModelState.AddModelError(nameof(vm.ProductIdToAdd), "Select a product.");

        if (vm.QuantityToAdd <= 0)
            ModelState.AddModelError(nameof(vm.QuantityToAdd), "Quantity must be > 0.");

        if (!ModelState.IsValid)
            return View("Create", vm);

        var allProducts = await productsApi.GetAllAsync(ct);
        var p = allProducts.FirstOrDefault(x => x.Id == vm.ProductIdToAdd);
        if (p is null)
        {
            ModelState.AddModelError(nameof(vm.ProductIdToAdd), "Product not found.");
            return View("Create", vm);
        }

        var existing = vm.InitialProducts?.FirstOrDefault(x => x.ProductId == p.Id);
        if (existing is null)
        {
            vm.InitialProducts?.Add(new InitialProductVm
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Quantity = vm.QuantityToAdd
            });
        }
        else
        {
            existing.Quantity += vm.QuantityToAdd;
        }

        vm.ProductIdToAdd = Guid.Empty;
        vm.QuantityToAdd = 1;

        return View("Create", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveInitialProduct(CreateFridgeVm vm, Guid productId, CancellationToken ct)
    {
        vm.InitialProducts ??= [];
        vm.InitialProducts?.RemoveAll(x => x.ProductId == productId);
        await PopulateCreateVmList(vm, ct);
        return View("Create", vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var fridge = await fridgesApi.GetByIdAsync(id, ct);

        if (fridge is null)
            return NotFound();

        var vm = new FridgeFormVm
        {
            Id = fridge.Id,
            Name = fridge.Name,
            OwnerName = fridge.OwnerName,
            ModelId = fridge.ModelId
        };

        await PopulateModel(vm, ct);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FridgeFormVm vm, CancellationToken ct)
    {
        if (vm.Id is null || vm.Id == Guid.Empty)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateModel(vm, ct);
            return View(vm);
        }

        await fridgesApi.UpdateAsync(vm.Id.Value, new UpdateFridgeRequest(vm.Name, vm.OwnerName, vm.ModelId), ct);

        TempData["Saved"] = "Saved";
        return RedirectToAction(nameof(Edit), new { id = vm.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await fridgesApi.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Products(Guid id, CancellationToken ct)
    {
        var fridge = await fridgesApi.GetByIdAsync(id, ct);
        if (fridge is null) return NotFound();

        var items = await fridgesApi.GetProductsAsync(id, ct);
        var products = await productsApi.GetAllAsync(ct);

        var vm = new FridgeProductsVm
        {
            FridgeId = id,
            FridgeName = fridge.Name,
            Items = items,
            Products = [.. products.Select(p => new SelectListItem(p.Name, p.Id.ToString()))]
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProduct(FridgeProductsVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Products), new { id = vm.FridgeId });

        await fridgesApi.AddProductAsync(vm.FridgeId, new AddProductToFridgeRequest(vm.ProductId, vm.Quantity), ct);
        return RedirectToAction(nameof(Products), new { id = vm.FridgeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(Guid fridgeId, Guid fridgeProductId, CancellationToken ct)
    {
        await fridgeProductApi.DeleteByIdAsync(fridgeProductId, ct);
        return RedirectToAction(nameof(Products), new { id = fridgeId });
    }

    private async Task PopulateModel(FridgeFormVm vm, CancellationToken ct)
    {
        var models = await fridgeModelsApi.GetAllAsync(ct);

        vm.Models = [.. models.Select(m => new SelectListItem(
                text: $"{m.Name} ({m.Year})",
                value: m.Id.ToString(),
                selected: m.Id == vm.ModelId
            ))];
    }

    private async Task PopulateCreateVmList(CreateFridgeVm vm, CancellationToken ct)
    {
        var models = await fridgeModelsApi.GetAllAsync(ct);

        vm.Models = [.. models.Select(m => new SelectListItem(
                text: $"{m.Name} ({m.Year})",
                value: m.Id.ToString(),
                selected: m.Id == vm.ModelId
            ))];

        var products = await productsApi.GetAllAsync(ct);

        vm.Products = [.. products.Select(x => new SelectListItem(
                x.Name,
                x.Id.ToString(),
                x.Id == vm.ProductIdToAdd
            ))];
    }
}