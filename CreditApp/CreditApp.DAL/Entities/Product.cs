using System.ComponentModel.DataAnnotations.Schema;
using CreditApp.DAL.Entities.Base;
using Microsoft.AspNetCore.Http;

namespace CreditApp.DAL.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public int Count { get; set; }
    public string Model { get; set; }
    public string Brand { get; set; }
    public string? FileName { get; set; }
    public Guid BranchId { get; set; }
    public Guid CategoryId { get; set; }
    public Branch? Branch { get; set; }
    public Category? Category { get; set; }
    [NotMapped]
    public IFormFile? File { get; set; }
    public ICollection<LoanItem> LoanItems { get; set; }

    public Product()
    {
        LoanItems = new List<LoanItem>();
    }
}