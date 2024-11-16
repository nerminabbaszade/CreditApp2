using CreditApp.BLL.Services.Interfaces;
using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.UI.Controllers;

public class ShopController : Controller
{
    private readonly IRepository<Product> _productRepository;
    private readonly ILayoutService _layoutService;
    private readonly IRepository<Loan> _loanRepository;
    private readonly UserManager<User> _userManager;

    public ShopController(IRepository<Product> productRepository, ILayoutService layoutService, UserManager<User> userManager, IRepository<Loan> loanRepository)
    {
        _productRepository = productRepository;
        _layoutService = layoutService;
        _userManager = userManager;
        _loanRepository = loanRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? categoryId)
    {
        ViewBag.Categories = await _layoutService.GetCategoriesAsync();

        var products = await _productRepository.GetAll(null)
            .Include(x=>x.Category).ThenInclude(x=>x.Parent)
            .ThenInclude(x=>x.Parent).Where(x=>x.CategoryId.ToString() == categoryId || x.Category.ParentId.ToString()==categoryId || x.Category.Parent.ParentId.ToString() == categoryId)
            .ToListAsync();
        
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string id)
    {
        var product =  await _productRepository.GetAll(x=>x.Id.ToString()==id)
            .Include(x=>x.Category)
            .Include(x=>x.Branch).ThenInclude(x=>x.Merchant).ThenInclude(x=>x.User).FirstOrDefaultAsync();
        
        if (product == null)
            return NotFound();
        
        return View(product);
    }

    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Cart()
    {
        var user = await _userManager.Users.Where(x=>x.UserName==User.Identity.Name).Include(x=>x.Customer).ThenInclude(x=>x.Loans)
            .ThenInclude(x=>x.LoanItems).ThenInclude(x=>x.Product).FirstOrDefaultAsync();
        
        Loan loan = user.Customer.Loans.Where(x=>x.IsActive && !x.IsApproved && !x.IsCustomerApproved).FirstOrDefault();

        return View(loan);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Cart(string id)
    {
       Loan? loan =  await _loanRepository.GetAll(x=>x.Id.ToString()==id)
           .Include(x=>x.LoanItems).ThenInclude(x=>x.Product)
           .FirstOrDefaultAsync();
              
       if (loan == null)
           return NotFound();
       
       foreach (var item in loan.LoanItems)
       {
           if (item.Count <= item.Product.Count)
           {
               item.Product.Count -= item.Count;
           }
           else
           {
               ModelState.AddModelError("",$"There are {item.Product.Count} {item.Product.Brand} {item.Product.Model} {item.Product.Name} in stock.You want to buy {item.Count}.Please try correct your order or wait for stock.");
               return View(loan);
           }
       }

       loan.IsCustomerApproved = true;
       _loanRepository.Update(loan);
       
       _productRepository.Update(loan.LoanItems.Select(x=>x.Product).ToList());
       
       return RedirectToAction("index","home");
       
    }
}