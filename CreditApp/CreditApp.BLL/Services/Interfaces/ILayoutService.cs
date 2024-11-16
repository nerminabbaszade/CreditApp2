using CreditApp.DAL.Entities;

namespace CreditApp.BLL.Services.Interfaces;

public interface ILayoutService
{
    Task<List<Category>> GetCategoriesAsync();
}