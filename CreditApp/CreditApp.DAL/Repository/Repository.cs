using System.Linq.Expressions;
using CreditApp.DAL.Context;
using CreditApp.DAL.Entities.Base;
using CreditApp.DAL.Exceptions;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.DAL.Repository;

public class Repository<T> : IRepository<T> where T : BaseEntity,new()
{
    private readonly CreditAppDbContext _context;
    private readonly DbSet<T> _dbSet;
    
    public Repository(CreditAppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public T Update(T entity)
    {
        _dbSet.Update(entity);
        _context.SaveChanges();
        return entity;
    }
    public void Update(List<T> entities)
    {
        _dbSet.UpdateRange(entities);
        _context.SaveChanges();
    }


    public T Delete(string id)
    {
        T? entity = _dbSet.Where(x=>x.Id.ToString() == id).FirstOrDefault();
        
        if (entity is null)
        {
            throw new EntityNotFoundException("Entity isn`t found");
        }
        
        _dbSet.Remove(entity);
        _context.SaveChanges();
        return entity;
    }

    public IQueryable<T> GetAll(Expression<Func<T, bool>>? predicate)
    {
        if (predicate is null)
        {
            return _dbSet.AsQueryable();
        }
        return _dbSet.Where(predicate);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.FirstOrDefaultAsync(predicate);
}