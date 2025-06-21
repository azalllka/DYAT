using DYAT.Application.Interfaces.Services;
using DYAT.Domain.Entities;
using DYAT.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DYAT.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ICartService _cartService;

    public OrderService(ApplicationDbContext context, ICartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public async Task<(bool Success, string ErrorMessage)> CreateOrderAsync(string userId)
    {
        var user = await _context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return (false, "Пользователь не найден");
        if (user.Wallet == null)
            return (false, "У пользователя нет кошелька");

        var cartItems = await _cartService.GetCartItemsAsync(userId);
        if (cartItems == null || !cartItems.Any())
            return (false, "Корзина пуста");

        var total = cartItems.Sum(i => i.TotalPrice);
        if (user.Wallet.Balance < total)
            return (false, "Недостаточно средств на балансе");

        // Создаём заказ
        var order = new Order
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            TotalAmount = total,
            Items = cartItems.Select(i => new OrderItem
            {
                ProductId = i.Product.Id,
                Quantity = i.Quantity,
                Price = i.Product.Price
            }).ToList()
        };
        _context.Orders.Add(order);

        // Списываем деньги
        user.Wallet.Balance -= total;

        // Очищаем корзину
        await _cartService.ClearCartAsync(userId);

        await _context.SaveChangesAsync();
        return (true, null);
    }
} 