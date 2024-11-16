using CreditApp.DAL.Entities.Base;

namespace CreditApp.DAL.Entities;

public class Merchant : BaseEntity
{
    public string Description  {get; set;}
    public string TerminalNo {get; set;}
    public string UserId {get; set;}
    public User User {get; set;}
    public ICollection<Branch> Branches {get; set;}

    public Merchant()
    {
        Branches = new List<Branch>();
    }
}