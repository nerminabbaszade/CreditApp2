using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.UI.Controllers;

[Authorize(Roles = "Customer")]
public class LoanController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IRepository<Loan> _loanRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<LoanItem> _loanItemRepository;
    
    public LoanController(UserManager<User> userManager, IRepository<Loan> loanRepository, IRepository<Product> productRepository, IRepository<LoanItem> loanItemRepository)
    {
        _userManager = userManager;
        _loanRepository = loanRepository;
        _productRepository = productRepository;
        _loanItemRepository = loanItemRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.Users.Where(x=>x.UserName==User.Identity.Name).Include(x=>x.Customer).FirstOrDefaultAsync();
        
        var loans = await _loanRepository
            .GetAll(x=>x.CustomerId == user.Customer.Id  && x.IsCustomerApproved)
            .Include(x=>x.LoanDetail)
            .Include(x=>x.LoanItems).ThenInclude(x=>x.Product)
            .OrderByDescending(x=>x.IsApproved)
            .ThenByDescending(x=>x.IsActive)
            .ToListAsync();
        
        
        return View(loans);
    }

    [HttpGet]
    public async Task<IActionResult> Add(string productId,int count)
    {
        var user = await _userManager.Users.Where(x=>x.UserName==User.Identity.Name).Include(x=>x.Customer).FirstOrDefaultAsync();
        
        var loan = await _loanRepository.GetAll(x=>x.CustomerId == user.Customer.Id && x.IsActive && !x.IsApproved && !x.IsCustomerApproved).Include(x=>x.LoanDetail).Include(x=>x.LoanItems).ThenInclude(x=>x.Product)
            .FirstOrDefaultAsync();
        
        var product = await _productRepository.GetAsync(x=>x.Id.ToString()==productId);
        
        if (product == null)
            throw new Exception("Product isn`t found");

        if (loan != null)
        {
            var loanItem = await _loanItemRepository.GetAsync(x => x.LoanId ==loan.Id && x.ProductId==product.Id);

            if (loanItem != null)
            {
                loanItem.Count++;
                loanItem.Price += product.Price;
                loan.LoanDetail.CurrentAmount += product.Price * count;
                loan.TotalPrice += product.Price * count;
                
                _loanItemRepository.Update(loanItem);
                _loanRepository.Update(loan);
            }
            else
            {
                loan.LoanItems.Add(new()
                {
                    Price = product.Price * count,
                    Count = count,
                    Product = product,
                });

                loan.LoanDetail.CurrentAmount += product.Price * count;
                loan.TotalPrice += product.Price * count;

                _loanRepository.Update(loan);
            }
        }
        
        else
        {
            Loan newLoan = new()
            {
                CustomerId = user?.Customer?.Id,
                IsActive = true,
                Title = "From Site",
                TotalPrice = product.Price * count,
                Terms = 5,
                IsApproved = false,
                LoanItems = new List<LoanItem>()
                {
                    new()
                    {
                        Price = product.Price * count,
                        Count = count,
                        Product = product,
                    }
                },
                LoanDetail = new LoanDetail()
                {
                    CurrentAmount = product.Price * count
                }
            };
            await _loanRepository.AddAsync(newLoan);
        }

        return RedirectToAction("cart","shop");
    }
    
    [HttpPost]
    public async Task<IActionResult> ChangeCount(string loanItemId,int count)
    {
        if (count < 1)
            return RedirectToAction("cart","shop");
        
        var item = await _loanItemRepository.GetAll(x => x.Id.ToString() == loanItemId).Include(x=>x.Product).Include(x=>x.Loan).ThenInclude(x=>x.LoanDetail).FirstOrDefaultAsync();
        
        if(item == null)
            return NotFound();
        
        
        int difference = item.Count - count;
        
        item.Count = count;
        item.Price += item.Product.Price * -difference;
        item.Loan.TotalPrice += item.Product.Price * -difference;
        item.Loan.LoanDetail.CurrentAmount += item.Product.Price * -difference;

        
        _loanItemRepository.Update(item);

        return RedirectToAction("cart","shop");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string loanItemId)
    {
        var item = await _loanItemRepository.GetAll(x => x.Id.ToString() == loanItemId).Include(x=>x.Loan).ThenInclude(x=>x.LoanDetail).FirstOrDefaultAsync();
        
        if(item is null)
            return RedirectToAction("cart","shop");

        item.Loan.TotalPrice -= item.Price;
        item.Loan.LoanDetail.CurrentAmount -= item.Price;

        _loanRepository.Update(item.Loan);
        _loanItemRepository.Delete(loanItemId);
        
        return RedirectToAction("cart","shop");
    }
}