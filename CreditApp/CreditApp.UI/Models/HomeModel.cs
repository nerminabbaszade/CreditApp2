using CreditApp.DAL.Entities;

namespace CreditApp.UI.Models;

public class HomeModel
{
    public ICollection<Product> Products { get; set; } = new List<Product>();
}