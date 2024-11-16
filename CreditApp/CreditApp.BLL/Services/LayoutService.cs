using CreditApp.BLL.Services.Interfaces;
using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.BLL.Services;

public class LayoutService : ILayoutService
{
    private readonly IRepository<Category> _categoryRepository;

    public LayoutService(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAll(x => x.Level == 1)
            .Include(x => x.Children).ThenInclude(x => x.Children).ThenInclude(x=>x.Products)
            .OrderBy(x=>x.Name)
            .ToListAsync();

        return categories;
    }
}