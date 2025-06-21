using DYAT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DYAT.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<IQueryable<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(int id);
    Task<IQueryable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IQueryable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<Product> AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
} 