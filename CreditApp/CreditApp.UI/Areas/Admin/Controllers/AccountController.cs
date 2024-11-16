using CreditApp.DAL.DTOs.Account;
using CreditApp.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CreditApp.UI.Areas.Admin.Controllers;

[Area("Admin")]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("index", "home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(Login model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        User? user = await _userManager.FindByNameAsync(model.Username);

        if (user is null)
        {
            ModelState.AddModelError("","Username or password is incorrect");
            return View(model);
        }

        if (await _userManager.IsInRoleAsync(user, "Customer"))
        {
            return BadRequest();
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("","Your account is locked. Please try again later");
                return View(model);
            }
            ModelState.AddModelError("","Username or password is incorrect");
            return View(model);
        }

        return RedirectToAction("index", "home");
    }
    [Authorize]
    public async Task<IActionResult> Update()
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        
        if(user is null)
            return NotFound();
       
        UpdateUser dto = new UpdateUser()
        {
            Username = user.UserName,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
        };
        return View(dto);
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(UpdateUser dto)
    {
        var user =  await _userManager.FindByNameAsync(User.Identity.Name);
        if (user is null)
        {
            return NotFound();
        }

        user.Email = dto.Email;
        user.Name = dto.Name;
        user.Surname = dto.Surname;
        user.UserName = dto.Username;


        var result = await _userManager.UpdateAsync(user);
        
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
            result =  await  _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(dto);
            }

        }

        await _signInManager.SignInAsync(user, true);

        return RedirectToAction("index", "home");
    }
    
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("index", "home");
    }
}