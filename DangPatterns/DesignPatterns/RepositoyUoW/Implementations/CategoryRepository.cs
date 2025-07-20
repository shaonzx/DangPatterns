using DangPatterns.DesignPatterns.RepositoyUoW.Interfaces;
using DangPatterns.Models;
using Microsoft.EntityFrameworkCore;

namespace DangPatterns.DesignPatterns.RepositoyUoW.Implementations
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Category?> GetCategoryWithProductsAsync(int id)
        {
            return await _dbSet.Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithProductCountAsync()
        {
            return await _dbSet.Include(c => c.Products).ToListAsync();
        }
    }
}
