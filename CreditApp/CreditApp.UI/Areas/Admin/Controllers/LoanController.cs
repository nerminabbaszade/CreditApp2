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
[Authorize(Roles = "SuperAdmin,Admin,Merchant,Employee")]
public class LoanController : Controller
{
    private readonly IRepository<Loan> _repository;
    private readonly UserManager<User> _userManager;

    public LoanController(IRepository<Loan> repository, UserManager<User> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }
    [HttpGet]
    public async Task<IActionResult> Index(string userNameOrId)
    {
        var loans = await _repository.GetAll(null)
            .Include(x=>x.Customer).ThenInclude(x=>x.User)
            .Include(x=>x.Employee).ThenInclude(x=>x.User)
            .Include(x=>x.LoanDetail)
            .Include(x=>x.LoanItems).Where(x=>(x.CustomerId.ToString() == userNameOrId && x.IsCustomerApproved) || (x.Employee.User.UserName == userNameOrId && x.IsCustomerApproved)).OrderByDescending(x=>!x.IsApproved)
            .ThenByDescending(x=>x.CreatedAt).ToListAsync();
        
        return View(loans);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string id)
    {
        var loan = await _repository.GetAll(null).Include(x=>x.Customer).ThenInclude(x=>x.User).Include(x=>x.Employee).ThenInclude(x=>x.User).Include(x=>x.LoanDetail).Include(x=>x.LoanItems).ThenInclude(x=>x.Product).Where(x=>x.Id.ToString()==id).FirstOrDefaultAsync();
        
        return View(loan);
    }
    
    [HttpGet]
    public async Task<IActionResult> Confirm(string id)
    {
        var loan = await _repository.GetAsync(x => x.Id.ToString() == id);

        if (loan is null)
        {
            return NotFound();
        }
        loan.IsApproved = true;
        loan.Employee =  (await _userManager.Users.Include(x=>x.Employee).FirstOrDefaultAsync(x=>x.UserName == User.Identity.Name)).Employee;
        
        _repository.Update(loan);
        
        return RedirectToAction("index","customer");
    }
}