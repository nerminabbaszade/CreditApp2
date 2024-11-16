using CreditApp.DAL.Entities.Base;

namespace CreditApp.DAL.Repository.Interfaces;

public interface IRepository<T> where T : BaseEntity,new()
{
    Task<T> AddAsync(T entity);
    T Update(T entity);
    void Update(List<T> entities);
    T Delete(string id);
    IQueryable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>>? predicate);
    Task<T?> GetAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
}