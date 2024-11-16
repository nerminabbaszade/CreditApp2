using CreditApp.DAL.Entities.Base;

namespace CreditApp.DAL.Entities;

public class Branch : BaseEntity
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
    public Guid MerchantId { get; set; }
    public Merchant? Merchant { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<Employee> Employees { get; set; }

    public Branch()
    {
        Employees = new List<Employee>();
        Products = new List<Product>();
    }
}