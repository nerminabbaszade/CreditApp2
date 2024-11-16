using CreditApp.DAL.DTOs.Customers;
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
public class CustomerController : Controller
{
    private readonly IRepository<Customer> _repository;
    private readonly UserManager<User> _userManager;

    public CustomerController(IRepository<Customer> repository, UserManager<User> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin,Merchant,Employee")]
    public async Task<IActionResult> Index()
    {
        var merchants = await _repository.GetAll(null).Include(x => x.User).Include(x=>x.Loans).ToListAsync();

        return View(merchants);
    }

    [HttpGet]
    [Authorize(Roles = "Employee")]
    public IActionResult Add()
    {
        return View();
    }
    
    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Add(CustomerPost dto)
    {
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

            Customer customer = new()
            {
                User = user,
                Address = dto.Address,
                Phone = dto.Phone,
                Occupation = dto.Occupation,
            };

            await _userManager.AddToRoleAsync(user, "Customer");
            
            await _repository.AddAsync(customer);
        }
        catch (Exception e)
        {
            return View(dto);
        }
        
        return RedirectToAction("index");
    }
    
    [HttpGet]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Update(string id)
    {
        var customer = await _repository.GetAll(x => x.Id.ToString() == id).Include(x=>x.User).FirstOrDefaultAsync();
        
        if (customer is null)
            return NotFound();

        CustomerUpdate dto = new()
        {
            Name = customer.User.Name,
            Surname = customer.User.Surname,
            Username = customer.User.UserName,
            Email = customer.User.Email,
            Phone= customer.Phone,
            Occupation = customer.Occupation,
            Address = customer.Address,
        };

        return View(dto);
    }
    
    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Update(string id, CustomerUpdate dto)
    {
        var model = _repository.GetAll(x => x.Id.ToString() == id).Include(x=>x.User).FirstOrDefault();
        
        if (model is null )
        {
            return NotFound();
        }

        model.User.Email = dto.Email;
        model.User.Name = dto.Name;
        model.User.Surname = dto.Surname;
        model.User.UserName = dto.Username;
        model.Address = dto.Address;
        model.Phone = dto.Phone;
        model.Occupation = dto.Occupation;

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
        
        _repository.Update(model);

        return RedirectToAction("index");
    }
    
    [Authorize(Roles = "Employee")]
    public IActionResult Delete(string id)
    {
        _repository.Delete(id);
        return RedirectToAction("index");
    }
    
}