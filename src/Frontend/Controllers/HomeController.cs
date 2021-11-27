using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;

namespace Frontend.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var toppings = new List<ToppingViewModel>
        {
            new("cheese", "Cheese", 1m),
            new("tomatosauce", "Tomato Sauce", 0.5m),
        };
        var crusts = new List<CrustViewModel>
        {
            new("thin9", "Thin", 9, 5m),
            new("deep9", "Deep", 9, 6m),
        };
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