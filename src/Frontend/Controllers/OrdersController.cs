using Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers;

[Route("orders")]
public class OrdersController : Controller
{
    private readonly ILogger<OrdersController> _log;

    public OrdersController(ILogger<OrdersController> log)
    {
        _log = log;
    }

    [HttpPost]
    public async Task<IActionResult> Order([FromForm]HomeViewModel viewModel)
    {
        return View();
    }
}