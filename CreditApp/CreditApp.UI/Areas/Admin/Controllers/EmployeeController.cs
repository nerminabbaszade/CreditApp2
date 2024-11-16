using CreditApp.DAL.DTOs.Employees;
using CreditApp.DAL.DTOs.Merchants;
using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.UI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SuperAdmin,Merchant")]
public class EmployeeController : Controller
{
    private readonly IRepository<Employee> _repository;
    private readonly UserManager<User> _userManager;
    private readonly IRepository<Branch> _branchRepository;

    public EmployeeController(IRepository<Employee> repository, UserManager<User> userManager, IRepository<Branch> branchRepository)
    {
        _repository = repository;
        _userManager = userManager;
        _branchRepository = branchRepository;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var employees = _repository.GetAll(null).Include(x => x.User).Include(x => x.Branch)
            .ThenInclude(x => x.Merchant).ThenInclude(x => x.User).AsQueryable();
        
        if (User.IsInRole("Merchant"))
        {
            var userId = (await _userManager.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefaultAsync()).Id;
            
            employees = employees.Where(x => x.Branch.Merchant.UserId == userId);
        }

        return View(employees.ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        ViewBag.Branches = await _branchRepository.GetAll(null).ToListAsync();
        if (User.IsInRole("Merchant"))
        {
            var userId = (await _userManager.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefaultAsync()).Id;
            
            ViewBag.Branches = await _branchRepository.GetAll(null).Include(x=>x.Merchant).ThenInclude(x=>x.User).Where(x => x.Merchant.UserId.ToString() == userId).ToListAsync();
        }
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Add(EmployeePost dto)
    {
        ViewBag.Branches = await _branchRepository.GetAll(null).ToListAsync();
        if (User.IsInRole("Merchant"))
        {
            var userId = (await _userManager.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefaultAsync()).Id;
            
            ViewBag.Branches = await _branchRepository.GetAll(null).Include(x=>x.Merchant).ThenInclude(x=>x.User).Where(x => x.Merchant.UserId.ToString() == userId).ToListAsync();
        }
        
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        try
        {
            User user = new()
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                UserName = dto.Username,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(dto);
            }

            Employee employee = new()
            {
                User = user,
                BranchId = Guid.Parse(dto.BranchId),
                Position = dto.Position,
            };

            await _userManager.AddToRoleAsync(user, "Employee");
            
            await _repository.AddAsync(employee);
        }
        catch (Exception e)
        {
            return View(dto);
        }
        
        return RedirectToAction("index");
    }
    
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        ViewBag.Branches = await _branchRepository.GetAll(null).ToListAsync();
        if (User.IsInRole("Merchant"))
        {
            var userId = (await _userManager.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefaultAsync()).Id;
            
            ViewBag.Branches = await _branchRepository.GetAll(null).Include(x=>x.Merchant).ThenInclude(x=>x.User).Where(x => x.Merchant.UserId.ToString() == userId).ToListAsync();
        }
        
        var merchant = await _repository.GetAll(x => x.Id.ToString() == id).Include(x=>x.User).FirstOrDefaultAsync();
        if (merchant is null)
            return NotFound();

        EmployeeUpdate dto = new()
        {
            Name = merchant.User.Name,
            Surname = merchant.User.Surname,
            Username = merchant.User.UserName,
            Email = merchant.User.Email,
            Position= merchant.Position,
            BranchId = merchant.BranchId.ToString(),
        };

        return View(dto);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(string id, EmployeeUpdate dto)
    {
        ViewBag.Branches = await _branchRepository.GetAll(null).ToListAsync();
        
        if (User.IsInRole("Merchant"))
        {
            var userId = (await _userManager.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefaultAsync()).Id;
            
            ViewBag.Branches = await _branchRepository.GetAll(null).Include(x=>x.Merchant).ThenInclude(x=>x.User).Where(x => x.Merchant.UserId.ToString() == userId).ToListAsync();
        }
        
        var model = _repository.GetAll(x => x.Id.ToString() == id).Include(x=>x.User).FirstOrDefault();
        
        if (model is null )
        {
            return NotFound();
        }

        model.User.Email = dto.Email;
        model.User.Name = dto.Name;
        model.User.Surname = dto.Surname;
        model.User.UserName = dto.Username;


        var result = await _userManager.UpdateAsync(model.User);
        
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(dto);
        }

        if (!string.IsNullOrWhiteSpace(dto.Password) && !string.IsNullOrWhiteSpace(dto.CurrentPassword))
        {
            result =  await  _userManager.ChangePasswordAsync(model.User, dto.CurrentPassword, dto.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(dto);
            }

        }
        
        model.BranchId = Guid.Parse(dto.BranchId);
        model.Position = dto.Position;
        _repository.Update(model);

        return RedirectToAction("index");
    }

    public IActionResult Delete(string id)
    {
        _repository.Delete(id);
        return RedirectToAction("index");
    }
    
}