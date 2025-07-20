using DangPatterns.DesignPatterns.RepositoyUoW.Interfaces;
using DangPatterns.Models;
using Microsoft.EntityFrameworkCore;

namespace DangPatterns.DesignPatterns.RepositoyUoW.Implementations
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _dbSet.Where(p => p.CategoryId == categoryId).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetExpensiveProductsAsync(decimal minPrice)
        {
            return await _dbSet.Where(p => p.Price >= minPrice).ToListAsync();
        }

        public async Task<Product?> GetProductWithCategoryAsync(int id)
        {
            return await _dbSet.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
