using DYAT.Domain.Models;

namespace DYAT.Application.Interfaces.Services;

public interface ICartService
{
    Task<List<CartItem>> GetCartItemsAsync(string userId);
    Task AddToCartAsync(string userId, int productId, int quantity);
    Task RemoveFromCartAsync(string userId, int productId);
    Task UpdateQuantityAsync(string userId, int productId, int quantity);
    Task ClearCartAsync(string userId);
    Task<decimal> GetTotalPriceAsync(string userId);
} 