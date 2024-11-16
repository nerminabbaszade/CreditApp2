using System.Diagnostics;
using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository;
using CreditApp.DAL.Repository.Interfaces;
using CreditApp.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.UI.Controllers;

public class HomeController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IRepository<Product> _productRepository;

    public HomeController(RoleManager<IdentityRole> roleManager, IRepository<Product> productRepository)
    {
        _roleManager = roleManager;
        _productRepository = productRepository;
    }

    public async Task<IActionResult> Index()
    {
        HomeModel model = new HomeModel()
        {
            Products = await _productRepository.GetAll(null).ToListAsync()
        };
        
        return View(model);
    }

}