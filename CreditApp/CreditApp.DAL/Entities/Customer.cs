using CreditApp.DAL.Entities.Base;

namespace CreditApp.DAL.Entities;

public class Customer : BaseEntity
{
    public string Occupation { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public ICollection<Loan> Loans { get; set; }

    public Customer()
    {
        Loans = new HashSet<Loan>();
    }
}