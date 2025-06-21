using DYAT.Application.Interfaces.Repositories;
using DYAT.Domain.Entities;
using DYAT.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DYAT.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Product>> GetAllProductsAsync()
    {
        return _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive);
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
    }

    public async Task<IQueryable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.IsActive);
    }

    public async Task<IQueryable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.Price >= minPrice && p.Price <= maxPrice);
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            product.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }
} 