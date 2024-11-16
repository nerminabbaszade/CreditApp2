using Microsoft.AspNetCore.Mvc;

namespace CreditApp.UI.Controllers;

public class BlogController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}