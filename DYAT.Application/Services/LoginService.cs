using Microsoft.AspNetCore.Identity;
using DYAT.Domain.Entities;
using DYAT.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace DYAT.Application.Services;

public class LoginService : ILoginService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<LoginService> _logger;

    public LoginService(
        SignInManager<User> signInManager, 
        UserManager<User> userManager,
        ILogger<LoginService> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<bool> LoginAsync(string email, string password, bool rememberMe)
    {
        try
        {
            _logger.LogInformation("Попытка входа пользователя с email: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Попытка входа с несуществующим email: {Email}", email);
                return false;
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("Успешный вход пользователя: {Email}", email);
                return true;
            }
            
            if (result.IsLockedOut)
            {
                _logger.LogWarning("Аккаунт заблокирован: {Email}", email);
                return false;
            }
            
            if (result.RequiresTwoFactor)
            {
                _logger.LogInformation("Требуется двухфакторная аутентификация: {Email}", email);
                return false;
            }

            _logger.LogWarning("Неудачная попытка входа пользователя: {Email}", email);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при попытке входа пользователя: {Email}", email);
            throw;
        }
    }
} 