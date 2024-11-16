using CreditApp.DAL.Entities.Base;

namespace CreditApp.DAL.Entities;

public class Employee : BaseEntity
{
    public string Position {get; set;}
    public string UserId {get; set;}
    public Guid BranchId {get; set;}
    public Branch Branch {get; set;}
    public User User {get; set;}
    public ICollection<Loan> Loans {get; set;}

    public Employee()
    {
        Loans = new List<Loan>();
    }
}