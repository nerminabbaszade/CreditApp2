using CreditApp.DAL.Entities.Base;

namespace CreditApp.DAL.Entities;

public class LoanItem : BaseEntity
{
    public Guid LoanId { get; set; }
    public Guid ProductId { get; set; }
    public Loan Loan { get; set; }
    public int Count { get; set; }
    public double Price { get; set; }
    
    public Product Product { get; set; }
}