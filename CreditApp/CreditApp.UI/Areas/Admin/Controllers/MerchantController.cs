using CreditApp.DAL.DTOs.Merchants;
using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.UI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class MerchantController : Controller
{
    private readonly IRepository<Merchant> _repository;
    private readonly UserManager<User> _userManager;

    public MerchantController(IRepository<Merchant> repository, UserManager<User> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var merchants = await _repository.GetAll(null).Include(x => x.User).Include(x=>x.Branches).ToListAsync();

        return View(merchants);
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Add(MerchantPost dto)
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

            Merchant merchant = new()
            {
                User = user,
                Description = dto.Description,
                TerminalNo = dto.TerminalNo,
            };

            await _userManager.AddToRoleAsync(user, "Merchant");
            
            await _repository.AddAsync(merchant);
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
        var merchant = await _repository.GetAll(x => x.Id.ToString() == id).Include(x=>x.User).FirstOrDefaultAsync();
        if (merchant is null)
            return NotFound();

        MerchantUpdate dto = new()
        {
            Name = merchant.User.Name,
            Surname = merchant.User.Surname,
            Username = merchant.User.UserName,
            Email = merchant.User.Email,
            Description = merchant.Description,
            TerminalNo = merchant.TerminalNo,
        };

        return View(dto);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(string id, MerchantUpdate dto)
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
        
        model.Description = dto.Description;
        model.TerminalNo = dto.TerminalNo;
        _repository.Update(model);

        return RedirectToAction("index");
    }

    public IActionResult Delete(string id)
    {
        _repository.Delete(id);
        return RedirectToAction("index");
    }
    
}