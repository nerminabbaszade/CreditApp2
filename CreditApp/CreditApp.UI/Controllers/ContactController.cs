using Microsoft.AspNetCore.Mvc;

namespace CreditApp.UI.Controllers;

public class ContactController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}