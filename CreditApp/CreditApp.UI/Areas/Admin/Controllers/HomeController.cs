using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditApp.UI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SuperAdmin,Merchant,Employee")]
public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}