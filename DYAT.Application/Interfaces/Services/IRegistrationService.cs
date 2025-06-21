using DYAT.Domain.DTO;

namespace DYAT.Application.Interfaces.Services;

public interface IRegistrationService
{
    Task<bool> RegisterUserAsync(string email, string password, string userName);
} 