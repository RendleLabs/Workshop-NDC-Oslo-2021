using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using Ingredients.Protos;

namespace Frontend.Controllers;

public class HomeController : Controller
{
    private readonly IngredientsService.IngredientsServiceClient _ingredientsClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IngredientsService.IngredientsServiceClient ingredientsClient,
        ILogger<HomeController> logger)
    {
        _ingredientsClient = ingredientsClient;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var toppingsTask = GetToppingViewModelsAsync();

        var crustsTask = GetCrustViewModelsAsync();

        await Task.WhenAll(toppingsTask, crustsTask);

        var toppings = toppingsTask.Result;
        var crusts = crustsTask.Result;

        var viewModel = new HomeViewModel(toppings, crusts);
        return View(viewModel);
    }

    private async Task<List<CrustViewModel>> GetCrustViewModelsAsync()
    {
        var crustsResponse = await _ingredientsClient.GetCrustsAsync(new GetCrustsRequest());

        var crusts = crustsResponse.Crusts
            .Select(c => new CrustViewModel(c.Id, c.Name, c.Size, c.Price))
            .ToList();
        return crusts;
    }

    private async Task<List<ToppingViewModel>> GetToppingViewModelsAsync()
    {
        var toppingsResponse = await _ingredientsClient.GetToppingsAsync(new GetToppingsRequest());

        var toppings = toppingsResponse.Toppings
            .Select(t => new ToppingViewModel(t.Id, t.Name, t.Price))
            .ToList();
        return toppings;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}