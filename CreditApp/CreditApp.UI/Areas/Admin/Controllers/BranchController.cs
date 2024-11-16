using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.UI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SuperAdmin,Merchant")]
public class BranchController : Controller
{
    private readonly IRepository<Branch> _repository;
    private readonly IRepository<Merchant> _repositoryMerchant;
    private readonly UserManager<User> _userManager;

    public BranchController(IRepository<Branch> repository, IRepository<Merchant> repositoryMerchant, UserManager<User> userManager)
    {
        _repository = repository;
        _repositoryMerchant = repositoryMerchant;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var branches = _repository.GetAll(null)
            .Include(x => x.Employees).Include(x => x.Merchant).ThenInclude(x => x.User).AsQueryable();
        
        if (User.IsInRole("Merchant"))
        {
            var userId = (await _userManager.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefaultAsync()).Id;
            branches = branches.Where(x => x.Merchant.UserId.ToString() == userId);
        }

        return View(await branches.ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        ViewBag.Merchants = await _repositoryMerchant.GetAll(null).Include(x=>x.User).ToListAsync();

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Add(Branch branch)
    {
        ViewBag.Merchants = await _repositoryMerchant.GetAll(null).Include(x=>x.User).ToListAsync();

        if (!ModelState.IsValid)
            return View(branch);

        await _repository.AddAsync(branch);
        return RedirectToAction("index");
    }
    
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        ViewBag.Merchants = await _repositoryMerchant.GetAll(null).Include(x=>x.User).ToListAsync();

        var branch = await _repository.GetAsync(x => x.Id.ToString() == id);
        if (branch is null)
            return NotFound();

        return View(branch);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(string id, Branch branch)
    {
        ViewBag.Merchants = await _repositoryMerchant.GetAll(null).Include(x=>x.User).ToListAsync();

        
        if (!ModelState.IsValid)
            return View(branch);

        var updatedBranch = await _repository.GetAsync(x => x.Id.ToString() == id);
        

        updatedBranch.Name = branch.Name;
        updatedBranch.Description = branch.Description;
        updatedBranch.Address = branch.Address;
        updatedBranch.MerchantId = branch.MerchantId;

        _repository.Update(updatedBranch);

        return RedirectToAction("index");
    }

    public IActionResult Delete(string id)
    {
        _repository.Delete(id);
        return RedirectToAction("index");
    }
    
}