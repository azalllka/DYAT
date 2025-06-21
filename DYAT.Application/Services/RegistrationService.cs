using DYAT.Application.Interfaces.Repositories;
using DYAT.Application.Interfaces.Services;
using DYAT.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DYAT.Application.Services;

public class RegistrationService : IRegistrationService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(UserManager<User> userManager, ILogger<RegistrationService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<bool> RegisterUserAsync(string email, string password, string userName)
    {
        try
        {
            _logger.LogInformation("Начало регистрации пользователя: {Email}", email);
            
            var user = new User
            {
                UserName = userName,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("Пользователь успешно зарегистрирован: {Email}", email);
                return true;
            }
            
            foreach (var error in result.Errors)
            {
                _logger.LogError("Ошибка при регистрации пользователя {Email}: {Error}", email, error.Description);
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при регистрации пользователя {Email}", email);
            throw;
        }
    }
} 