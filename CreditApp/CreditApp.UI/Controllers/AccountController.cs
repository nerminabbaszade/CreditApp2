using CreditApp.BLL.Services.Interfaces;
using CreditApp.DAL.DTOs.Account;
using CreditApp.DAL.DTOs.Customers;
using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.UI.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IMailService _mailService;
    private readonly IRepository<Customer> _repository;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IMailService mailService,
        IRepository<Customer> repository, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mailService = mailService;
        _repository = repository;
        _roleManager = roleManager;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(CustomerPost model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        User user = new()
        {
            Name = model.Name,
            Surname = model.Surname,
            Email = model.Email,
            UserName = model.Username,
            Customer = new Customer()
            {
                Address = model.Address,
                Occupation = model.Occupation,
                Phone = model.Phone,
            }
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        await _userManager.AddToRoleAsync(user, "Customer");
        
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var link = Url.Action("VerifyEmail", controller: "Account", values: new
        {
            token = token,
            mail = user.Email
        }, protocol: Request.Scheme);

        _mailService.SendMail(user.Email, "Verification Email", $"Your account activation link: {link}");

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(Login model)
    {
        User? user = await _userManager.FindByNameAsync(model.Username);

        if (user is null)
        {
            ModelState.AddModelError(" ", "Username or password is incorrect");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(" ", "Your account is locked. Please try again later");
                return View(model);
            }

            ModelState.AddModelError(" ", "Username or password is incorrect");
            return View(model);
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> VerifyEmail(string token, string mail)
    {
        var user = await _userManager.FindByEmailAsync(mail);
        if (user is null)
        {
            NotFound();
        }

        await _userManager.ConfirmEmailAsync(user, token);
        await _signInManager.SignInAsync(user, true);
        return RedirectToAction("Index", "Home");
    }

    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Update()
    {
        var user = await _userManager.Users.Where(x => x.UserName == User.Identity.Name).Include(x => x.Customer)
            .FirstOrDefaultAsync();
        if (user is null)
        {
            return NotFound();
        }

        CustomerUpdate model = new()
        {
            Username = user.UserName,
            Surname = user.Surname,
            Name = user.Name,
            Email = user.Email,
            Occupation = user.Customer.Occupation,
            Address = user.Customer.Address,
            Phone = user.Customer.Phone,
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Update(CustomerUpdate dto)
    {
        var model = (await _userManager.Users.Where(x => x.UserName == User.Identity.Name).Include(x => x.Customer)
            .FirstOrDefaultAsync()).Customer;

        if (model is null)
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
            result = await _userManager.ChangePasswordAsync(model.User, dto.CurrentPassword, dto.Password);
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

        return RedirectToAction("index", "home");
    }

    [HttpGet]
    public IActionResult ForgetPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgetPassword(ForgetPassword model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Mail))
        {
            return BadRequest();
        }

        var user = await _userManager.FindByEmailAsync(model.Mail);

        if (user is null)
        {
            return RedirectToAction("index", "home");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        string? link = Url.Action(action: "ResetPassword", controller: "Account", values: new
        {
            token,
            mail = model.Mail
        }, protocol: Request.Scheme);

        _mailService.SendMail(model.Mail, "Reset Password", $"For resetting your account password:{link}");
        return RedirectToAction("index", "home");
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string token, string mail)
    {
        var user = await _userManager.FindByEmailAsync(mail);
        if (user is null)
        {
            return NotFound();
        }

        ResetPassword model = new ResetPassword()
        {
            Mail = mail,
            Token = token
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPassword model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Mail);
        if (user is null)
        {
            return NotFound();
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                return View(model);
            }
        }

        return RedirectToAction("Login", "account");
    }

//    public async Task<IActionResult> AddRole()
//    {
//        await _roleManager.CreateAsync(new IdentityRole("Admin"));
//        await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
//        await _roleManager.CreateAsync(new IdentityRole("Merchant"));
//        await _roleManager.CreateAsync(new IdentityRole("Customer"));
//        await _roleManager.CreateAsync(new IdentityRole("Employee"));
//        return Ok();
//    }

//    public async Task<IActionResult> AdminCreate()
//    {
//        User SuperAdmin = new User
//        {
//            Name = "SuperAdmin",
//            Surname = "SuperAdmin",
//            Email = "superadmin@orange.com",
//            UserName = "SuperAdmin",
//            EmailConfirmed = true
//        };
//        await _userManager.CreateAsync(SuperAdmin, "Admin123.");
//        User Admin = new User
//        {
//            Name = "Admin",
//            Surname = "Admin",
//            Email = "admin@orange.com",
//            UserName = "Admin",
//            EmailConfirmed = true
//        };
//        await _userManager.CreateAsync(Admin, "Admin123.");

//        await _userManager.AddToRoleAsync(SuperAdmin, "SuperAdmin");
//        await _userManager.AddToRoleAsync(Admin, "Admin");
//        return Json("ok");
//    }
}