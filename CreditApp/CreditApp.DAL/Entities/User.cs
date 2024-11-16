using Microsoft.AspNetCore.Identity;

namespace CreditApp.DAL.Entities;

public class User : IdentityUser
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public Merchant? Merchant { get; set; }
    public Employee? Employee { get; set; }
    public Customer? Customer { get; set; }
}