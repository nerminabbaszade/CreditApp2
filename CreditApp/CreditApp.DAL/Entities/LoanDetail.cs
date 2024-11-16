using CreditApp.DAL.Entities.Base;

namespace CreditApp.DAL.Entities;

public class LoanDetail : BaseEntity
{
    public double CurrentAmount { get; set; }
    public Guid LoanId { get; set; }
    public Loan Loan { get; set; }
}