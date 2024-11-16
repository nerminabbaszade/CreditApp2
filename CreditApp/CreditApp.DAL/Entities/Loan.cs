using CreditApp.DAL.Entities.Base;

namespace CreditApp.DAL.Entities;

public class Loan :BaseEntity
{
    public string Title { get; set; }
    public double MonthlyPrice { get; set; }
    public double TotalPrice { get; set; }
    public int Terms { get; set; }
    public bool IsActive { get; set; }
    public bool IsCustomerApproved { get; set; }
    public bool IsApproved { get; set; }
    public Guid? EmployeeId { get; set; }
    public Guid? CustomerId { get; set; }
    public Employee? Employee { get; set; }
    public Customer? Customer { get; set; }
    public LoanDetail LoanDetail { get; set; }
    public ICollection<Payment> Payments { get; set; }
    public ICollection<LoanItem> LoanItems { get; set; }

    public Loan()
    {
        Payments = new List<Payment>();
        LoanItems = new List<LoanItem>();
    }
}