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
        var toppingsResponse = await _ingredientsClient.GetToppingsAsync(new GetToppingsRequest());

        var toppings = toppingsResponse.Toppings
            .Select(t => new ToppingViewModel(t.Id, t.Name, t.Price))
            .ToList();

        var crustsResponse = await _ingredientsClient.GetCrustsAsync(new GetCrustsRequest());

        var crusts = crustsResponse.Crusts
            .Select(c => new CrustViewModel(c.Id, c.Name, c.Size, c.Price))
            .ToList();
        
        var viewModel = new HomeViewModel(toppings, crusts);
        return View(viewModel);
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