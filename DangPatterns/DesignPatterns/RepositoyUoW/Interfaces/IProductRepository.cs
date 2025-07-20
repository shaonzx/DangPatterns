using DangPatterns.Models;

namespace DangPatterns.DesignPatterns.RepositoyUoW.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetExpensiveProductsAsync(decimal minPrice);
        Task<Product?> GetProductWithCategoryAsync(int id);
    }
}
