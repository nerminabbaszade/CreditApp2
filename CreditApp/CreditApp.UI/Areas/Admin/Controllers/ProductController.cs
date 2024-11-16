using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.UI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SuperAdmin,Employee,Merchant")]
public class ProductController : Controller
{
    private readonly IRepository<Product> _repository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UserManager<User> _userManager;

    public ProductController(IRepository<Product> repository, IRepository<Category> categoryRepository, IWebHostEnvironment webHostEnvironment, UserManager<User> userManager)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _webHostEnvironment = webHostEnvironment;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = _repository.GetAll(null).Include(x=>x.Branch).ThenInclude(x=>x.Merchant).ThenInclude(x=>x.User).Include(x => x.Category).AsQueryable();
        
        if (User.IsInRole("Employee"))
        {
            var branchId = _userManager.Users.Where(x => x.UserName == User.Identity.Name).Include(x => x.Employee)
                .FirstOrDefault().Employee.BranchId;
            
            products = products.Where(x=>x.BranchId == branchId);
        }
        else if (User.IsInRole("Merchant"))
        {
            var merchatId = _userManager.Users.Where(x => x.UserName == User.Identity.Name).Include(x => x.Merchant)
                .FirstOrDefault().Merchant.Id;
            
            products = products.Where(x => x.Branch.MerchantId == merchatId);
        }
        
        return View(await products.ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        ViewBag.Categories = await _categoryRepository.GetAll(x=>x.Level==3).Include(x=>x.Parent).ThenInclude(x=>x.Parent).OrderBy(x=>x.Parent.Parent.Name).ThenBy(x=>x.Parent.Name)
            .ThenBy(x=>x.Name).ToListAsync();

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Add(Product product)
    {
        ViewBag.Categories = await _categoryRepository.GetAll(x=>x.Level==3).Include(x=>x.Parent).ThenInclude(x=>x.Parent).OrderBy(x=>x.Parent.Parent.Name).ThenBy(x=>x.Parent.Name)
            .ThenBy(x=>x.Name).ToListAsync();

        if (product.File is null)
        {
            ModelState.AddModelError("File", "File is required");
            return View(product);
        }

        if (User.IsInRole("Employee"))
        {
            product.BranchId =
                (await _userManager.Users.Where(x => x.UserName == User.Identity.Name).Include(x => x.Employee)
                    .FirstOrDefaultAsync()).Employee.BranchId;
        }

        if (!ModelState.IsValid)
            return View(product);
        
        string fileName = Guid.NewGuid() + product.File.FileName;
        
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "images/products", fileName);

        using (FileStream stream = System.IO.File.Open(path, FileMode.CreateNew))
        {
            await product.File.CopyToAsync(stream);
        }

        product.FileName = fileName;

        await _repository.AddAsync(product);
        return RedirectToAction("index");
    }
    
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        ViewBag.Categories = await _categoryRepository.GetAll(x=>x.Level==3).Include(x=>x.Parent).ThenInclude(x=>x.Parent).OrderBy(x=>x.Parent.Parent.Name).ThenBy(x=>x.Parent.Name)
            .ThenBy(x=>x.Name).ToListAsync();
        
        var product = await _repository.GetAsync(x => x.Id.ToString() == id);
        if (product is null)
            return NotFound();

        return View(product);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(string id, Product product)
    {
        ViewBag.Categories = await _categoryRepository.GetAll(x=>x.Level==3).Include(x=>x.Parent).ThenInclude(x=>x.Parent).OrderBy(x=>x.Parent.Parent.Name).ThenBy(x=>x.Parent.Name)
            .ThenBy(x=>x.Name).ToListAsync();
 
        if (!ModelState.IsValid)
            return View(product);

        var updatedProduct = await _repository.GetAsync(x => x.Id.ToString() == id);
        
        if (updatedProduct is null)
            return NotFound();
        
        if (product.File != null)
        {
            string fileName = Guid.NewGuid() + product.File.FileName;
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "images/products", fileName);

            using (FileStream stream = System.IO.File.Open(path, FileMode.CreateNew))
            {
                await product.File.CopyToAsync(stream);
            }

            updatedProduct.FileName = fileName;
        }
        
        
        updatedProduct.Name = product.Name;
        updatedProduct.Price = product.Price;
        updatedProduct.Description = product.Description;
        updatedProduct.Count = product.Count;
        updatedProduct.Model = product.Model;
        updatedProduct.Brand = product.Brand;
        updatedProduct.CategoryId = product.CategoryId;

        _repository.Update(updatedProduct);

        return RedirectToAction("index");
    }

    public IActionResult Delete(string id)
    {
        _repository.Delete(id);
        return RedirectToAction("index");
    }
    
}