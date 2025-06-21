using DYAT.Application.Interfaces.Repositories;
using DYAT.Application.Interfaces.Services;
using DYAT.Domain.Entities;
using DYAT.Domain.Models;
using DYAT.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DYAT.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly IProductRepository _productRepository;
    private readonly ApplicationDbContext _context;

    public CartService(IProductRepository productRepository, ApplicationDbContext context)
    {
        _productRepository = productRepository;
        _context = context;
    }

    public async Task<List<Domain.Models.CartItem>> GetCartItemsAsync(string userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var items = await _context.CartItems
            .Include(ci => ci.Product)
            .Where(ci => ci.CartId == cart.Id)
            .ToListAsync();

        return items.Select(ci => new Domain.Models.CartItem
        {
            Product = ci.Product,
            Quantity = ci.Quantity
        }).ToList();
    }

    public async Task AddToCartAsync(string userId, int productId, int quantity)
    {
        var product = await _productRepository.GetProductByIdAsync(productId);
        if (product == null)
        {
            throw new ArgumentException("Товар не найден");
        }

        var cart = await GetOrCreateCartAsync(userId);
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var cartItem = new Domain.Entities.CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity
            };
            _context.CartItems.Add(cartItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(string userId, int productId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var item = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

        if (item != null)
        {
            _context.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateQuantityAsync(string userId, int productId, int quantity)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var item = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

        if (item != null)
        {
            item.Quantity = quantity;
            item.UpdatedAt = DateTime.UtcNow;
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(string userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var items = await _context.CartItems
            .Where(ci => ci.CartId == cart.Id)
            .ToListAsync();

        _context.CartItems.RemoveRange(items);
        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalPriceAsync(string userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var items = await _context.CartItems
            .Include(ci => ci.Product)
            .Where(ci => ci.CartId == cart.Id)
            .ToListAsync();

        return items.Sum(item => item.Product.Price * item.Quantity);
    }

    private async Task<Cart> GetOrCreateCartAsync(string userId)
    {
        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }
} 