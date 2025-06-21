using DYAT.Domain.DTO;
using DYAT.Domain.Entities;

namespace DYAT.Application.Interfaces.Services;

public interface ILoginService
{
    Task<bool> LoginAsync(string email, string password, bool rememberMe);
} 