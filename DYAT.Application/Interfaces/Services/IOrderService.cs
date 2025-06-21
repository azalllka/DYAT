using System.Threading.Tasks;

namespace DYAT.Application.Interfaces.Services;

public interface IOrderService
{
    Task<(bool Success, string ErrorMessage)> CreateOrderAsync(string userId);
} 