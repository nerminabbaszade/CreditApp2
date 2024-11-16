using System.ComponentModel.DataAnnotations.Schema;
using CreditApp.DAL.Entities.Base;

namespace CreditApp.DAL.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public int Level { get; set; }
    public Guid? ParentId { get; set; }
    [InverseProperty("Children")]
    public Category? Parent { get; set; }
    [InverseProperty("Parent")]
    public ICollection<Category> Children { get; set; }
    public ICollection<Product> Products { get; set; }

    public Category()
    {
        Children = new List<Category>();
        Products = new List<Product>();
    }
}