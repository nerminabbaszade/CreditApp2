using CreditApp.DAL.DTOs.Payment;
using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.UI.Controllers;

[Authorize(Roles = "Customer")]
public class PaymentController : Controller
{
    private readonly IRepository<Loan> _loanRepository;
    private readonly IRepository<Payment> _paymentRepository;

    public PaymentController(IRepository<Loan> loanRepository, IRepository<Payment> paymentRepository)
    {
        _loanRepository = loanRepository;
        _paymentRepository = paymentRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Pay(string loanId)
    {
        
        var loan = await _loanRepository.GetAll(x => x.Id.ToString() == loanId).Include(x=>x.LoanDetail).FirstOrDefaultAsync();
        
        if(loan is null || loan.LoanDetail.CurrentAmount<=0)
            return BadRequest();
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Pay(string loanId,PaymentPost paymentPost)
    {
        if (!ModelState.IsValid)
        {
            return View(paymentPost);
        }
        
        var loan = await _loanRepository.GetAll(x => x.Id.ToString() == loanId).Include(x=>x.LoanDetail).FirstOrDefaultAsync();
        
        if(loan is null)
            return BadRequest();
        
        if (loan.LoanDetail.CurrentAmount < paymentPost.Amount)
        {
            ModelState.AddModelError("Amount", $"Amount must be equal or less than {loan.LoanDetail.CurrentAmount}");
            return View(paymentPost);
        }

        Payment data = new()
        {
            Amount = paymentPost.Amount,
            Loan = loan,
            CreatedAt = DateTime.Now,
            PaymentType = "Cart",
        };
        
        loan.LoanDetail.CurrentAmount -= paymentPost.Amount;
        if (loan.LoanDetail.CurrentAmount == 0)
        {
            loan.IsActive = false;
        }
        
        await _paymentRepository.AddAsync(data);
        _loanRepository.Update(loan);
        
        return RedirectToAction("success", "payment", new { id = data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Success(string id)
    {
        var payment = await _paymentRepository.GetAsync(x=>x.Id.ToString() == id);
        return View(payment);
    }

    [HttpGet]
    public async Task<IActionResult> List(string loanId)
    {
        var payment = await _paymentRepository.GetAll(x=>x.LoanId.ToString() == loanId)
            .OrderByDescending(x=>x.CreatedAt)
            .ToListAsync();
        
        return View(payment);
    }
}