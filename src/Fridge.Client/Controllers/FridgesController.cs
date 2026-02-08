using Fridge.Client.Api.Clients;
using Fridge.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Fridge.Client.Api.Clients.FridgesApi;

namespace Fridge.Client.Controllers;

public class FridgesController(FridgesApi fridgesApi, FridgeModelsApi fridgeModelsApi) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var fridges = await fridgesApi.GetAllAsync(ct) ?? [];
        return View(fridges);
    }

    public async Task<IActionResult> Products(Guid id, CancellationToken ct)
    {
        var products = await fridgesApi.GetProductsAsync(id, ct) ?? [];
        ViewBag.FridgeId = id;
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var models = await fridgeModelsApi.GetAllAsync(ct);

        var vm = new FridgeFormVm
        {
            Models = [.. models.Select(m => new SelectListItem(
                text: $"{m.Name} ({m.Year})",
                value: m.Id.ToString()
            ))]
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FridgeFormVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateModel(vm, ct);
            return View(vm);
        }

        var id = await fridgesApi.CreateAsync(new CreateFridgeRequest(vm.Name, vm.OwnerName, vm.ModelId), ct);

        return RedirectToAction(nameof(Edit), new { id });//ToDo ?????
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

    private async Task PopulateModel(FridgeFormVm vm, CancellationToken ct)
    {
        var models = await fridgeModelsApi.GetAllAsync(ct);

        vm.Models = [.. models.Select(m => new SelectListItem(
                text: $"{m.Name} ({m.Year})",
                value: m.Id.ToString(),
                selected: m.Id == vm.ModelId
            ))];
    }
}