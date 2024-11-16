using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.UI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class CategoryController : Controller
{
    private readonly IRepository<Category> _repository;

    public CategoryController(IRepository<Category> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await _repository.GetAll(null).Include(x => x.Parent)
            .OrderBy(x=>x.Level).ToListAsync();

        return View(categories);
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        ViewBag.Categories = await _repository.GetAll(x => x.Level != 3).ToListAsync();
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Add(Category category)
    {
        ViewBag.Categories = await _repository.GetAll(x => x.Level != 3).ToListAsync();

        if (!ModelState.IsValid)
            return View(category);

        if (category.ParentId.HasValue)
        {
            var parent = await _repository.GetAsync(x => x.Id == category.ParentId.Value);
            if (parent is null)
                return NotFound();

            category.Level = parent.ParentId.HasValue ? 3 : 2;
        }
        else
        {
            category.Level = 1;
        }

        await _repository.AddAsync(category);
        return RedirectToAction("index");
    }
    
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        ViewBag.Categories = await _repository.GetAll(x => x.Level != 3 && x.Id.ToString() != id).ToListAsync();

        var category = await _repository.GetAsync(x => x.Id.ToString() == id);
        if (category is null)
            return NotFound();

        return View(category);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(string id, Category category)
    {
        ViewBag.Categories = await _repository.GetAll(x => x.Level != 3 && x.Id.ToString() != id).ToListAsync();
        if (!ModelState.IsValid)
            return View(category);

        var updatedCategory = await _repository.GetAsync(x => x.Id.ToString() == id);
        if (updatedCategory is null)
            return NotFound();

        if (category.ParentId.HasValue)
        {
            var parent = await _repository.GetAsync(x => x.Id == category.ParentId.Value);
            if (parent is null)
                return NotFound();

            updatedCategory.Level = parent.ParentId.HasValue ? 3 : 2;
        }
        else
        {
            updatedCategory.Level = 1;
        }

        updatedCategory.ParentId = category.ParentId;
        updatedCategory.Name = category.Name;
        _repository.Update(updatedCategory);

        return RedirectToAction("index");
    }

    public IActionResult Delete(string id)
    {
        _repository.Delete(id);
        return RedirectToAction("index");
    }
    
}