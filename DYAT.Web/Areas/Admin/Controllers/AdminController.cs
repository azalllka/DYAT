using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DYAT.Domain.Entities;
using DYAT.Application.Interfaces.Services;
using DYAT.Web.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;
using DYAT.Persistence.Context;

namespace DYAT.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AdminController> _logger;
    private readonly ApplicationDbContext _context;

    public AdminController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<AdminController> logger,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Попытка доступа к админ-панели неаутентифицированным пользователем");
                return Challenge();
            }

            var userRoles = await _userManager.GetRolesAsync(currentUser);
            _logger.LogInformation("Пользователь {Email} с ролями {Roles} пытается получить доступ к админ-панели",
                currentUser.Email, string.Join(", ", userRoles));

            if (!userRoles.Contains("Admin"))
            {
                _logger.LogWarning("Пользователь {Email} не имеет роли Admin", currentUser.Email);
                return Forbid();
            }

            var users = await _context.Users
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    IsLocked = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles,
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => r.Name)
                        .ToList()
                })
                .ToListAsync();

            _logger.LogInformation("Администратор {Email} успешно получил доступ к списку пользователей", currentUser.Email);
            return View(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка пользователей");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleUserLock(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Попытка блокировки несуществующего пользователя {UserId}", userId);
                return NotFound();
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                // Разблокировать пользователя
                await _userManager.SetLockoutEndDateAsync(user, null);
                _logger.LogInformation("Пользователь {UserId} разблокирован администратором {AdminId}", 
                    userId, User.Identity?.Name);
            }
            else
            {
                // Заблокировать пользователя на 30 дней
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(30));
                _logger.LogInformation("Пользователь {UserId} заблокирован администратором {AdminId}", 
                    userId, User.Identity?.Name);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при изменении статуса блокировки пользователя {UserId}", userId);
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Попытка удаления несуществующего пользователя {UserId}", userId);
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("Пользователь {UserId} удален администратором {AdminId}", 
                    userId, User.Identity?.Name);
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError("Ошибка при удалении пользователя {UserId}: {Error}", userId, error.Description);
            }

            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении пользователя {UserId}", userId);
            return View("Error");
        }
    }
} 