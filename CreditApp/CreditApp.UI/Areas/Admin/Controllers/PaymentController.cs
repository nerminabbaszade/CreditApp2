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
public class PaymentController : Controller
{
    private readonly IRepository<Payment> _repository;

    public PaymentController(IRepository<Payment> repository)
    {
        _repository = repository;
    }
    [HttpGet]
    public async Task<IActionResult> Index(string loanId)
    {
        var loans = await _repository.GetAll(null).OrderByDescending(x=>x.CreatedAt).Where(x=>x.LoanId.ToString() == loanId).ToListAsync();
        
        return View(loans);
    }
}