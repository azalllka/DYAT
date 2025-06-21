using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DYAT.Domain.Entities;
using DYAT.Web.Areas.Auth.Models;
using DYAT.Persistence.Context;
using DYAT.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace DYAT.Web.Areas.Auth.Controllers;

[Area("Auth")]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly ILoginService _loginService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ApplicationDbContext context,
        ILoginService loginService,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _loginService = loginService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            try
            {
                // Проверяем существование пользователя
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning("Попытка входа с несуществующим email: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Пользователь с таким email не найден.");
                    return View(model);
                }

                // Проверяем, не заблокирован ли пользователь
                if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
                {
                    _logger.LogWarning("Попытка входа заблокированным пользователем: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Ваш аккаунт заблокирован. Пожалуйста, обратитесь к администратору.");
                    return View(model);
                }

                var result = await _loginService.LoginAsync(model.Email, model.Password, model.RememberMe);
                if (result)
                {
                    _logger.LogInformation("Успешный вход пользователя {Email}", model.Email);
                    return RedirectToLocal(returnUrl);
                }

                _logger.LogWarning("Неудачная попытка входа пользователя {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Неверный пароль.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе пользователя {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Произошла ошибка при попытке входа. Пожалуйста, попробуйте позже.");
            }
        }
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            var user = new User { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var userWithWallet = await _context.Users
            .Include(u => u.Wallet)
            .Include(u => u.Orders)
                .ThenInclude(o => o.Items)
                    .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        if (userWithWallet == null)
        {
            return NotFound();
        }

        var model = new ProfileViewModel
        {
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Balance = userWithWallet.Wallet?.Balance ?? 0,
            Orders = userWithWallet.Orders ?? new List<Order>()
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home", new { area = "" });
    }
} 