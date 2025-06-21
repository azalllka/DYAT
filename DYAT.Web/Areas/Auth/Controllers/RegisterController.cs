using DYAT.Application.Interfaces.Services;
using DYAT.Domain.DTO;
using DYAT.Web.Areas.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace DYAT.Web.Areas.Auth.Controllers;

[Area("Auth")]
[AllowAnonymous]
public class RegisterController : Controller
{
    private readonly IRegistrationService _registrationService;

    public RegisterController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromForm] RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _registrationService.RegisterUserAsync(model.Email, model.Password, model.UserName);
                if (result)
                {
                    return RedirectToAction("Index", "Home", new { Area = "" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ошибка при регистрации пользователя. Возможно, такой email или имя пользователя уже существуют.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        return View(model);
    }
}