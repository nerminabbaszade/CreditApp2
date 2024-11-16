using CreditApp.DAL.Entities.Base;

namespace CreditApp.DAL.Entities;

public class Payment : BaseEntity
{
    public double Amount { get; set; }
    public string PaymentType { get; set; }
    public Guid LoanId { get; set; }
    public Loan Loan { get; set; }
}