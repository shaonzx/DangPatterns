using DangPatterns.Models;

namespace DangPatterns.DesignPatterns.RepositoyUoW.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetCategoryWithProductsAsync(int id);
        Task<IEnumerable<Category>> GetCategoriesWithProductCountAsync();
    }
}
