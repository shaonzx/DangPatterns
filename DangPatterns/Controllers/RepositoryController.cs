using DangPatterns.DesignPatterns.RepositoyUoW.Interfaces;
using DangPatterns.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DangPatterns.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RepositoryController> _logger;

        public RepositoryController(IUnitOfWork unitOfWork, ILogger<RepositoryController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet("CreateCategory")]
        public async Task<IActionResult> CreateCategory(string name)
        {
            var cat = new Category
            {
                Name = name
            };

            await _unitOfWork.Categories.AddAsync(cat);
            await _unitOfWork.SaveChangesAsync();

            return Ok(cat);
        }

        [HttpGet("CreateProduct")]
        public async Task<IActionResult> CreateProduct(string name, decimal price, int categoryId)
        {
            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero");

            var categoryExists = await _unitOfWork.Categories.ExistsAsync(categoryId);
            if (!categoryExists)
                throw new ArgumentException("Category does not exist");

            var product = new Product
            {
                Name = name,
                Price = price,
                CategoryId = categoryId
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return Ok(product);
        }

        [HttpGet("RenameCategoryAndProductInTransaction")]
        public async Task<IActionResult> CreateProduct(int categoryId, string newCategoryName, int productId, string newProductName)
        {

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cat = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                var prod = await _unitOfWork.Products.GetByIdAsync(productId);

                cat.Name = newCategoryName;
                prod.Name = newProductName;
                _unitOfWork.Categories.Update(cat);
                _unitOfWork.Products.Update(prod);

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(e.Message);
            }

            return Ok("The transaction was successful");
        }
    }
}
