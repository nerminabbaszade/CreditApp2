using CreditApp.DAL.Entities;
using CreditApp.DAL.Entities.Base;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.DAL.Context;

public class CreditAppDbContext : IdentityDbContext<User>
{
    public CreditAppDbContext(DbContextOptions<CreditAppDbContext> options) : base(options)
    {
        
    }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<LoanDetail> LoanDetails { get; set; }
    public DbSet<LoanItem> LoanItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Product> Products { get; set; }
    
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var models = ChangeTracker.Entries<BaseEntity>();

        foreach (var model in models)
        {
            if (model.State == EntityState.Added)
                model.Entity.CreatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}