using Microsoft.AspNetCore.Identity;
using DYAT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DYAT.Web.Data;

public static class RoleInitializer
{
    public static async Task InitializeAsync(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger logger)
    {
        string adminEmail = "admin@dyat.com";
        string adminPassword = "Admin123!";

        // Проверяем существующие роли
        var existingRoles = await roleManager.Roles.ToListAsync();
        logger.LogInformation("Существующие роли: {Roles}", string.Join(", ", existingRoles.Select(r => r.Name)));

        if (await roleManager.FindByNameAsync("Admin") == null)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            logger.LogInformation("Создана роль Admin");
        }

        if (await roleManager.FindByNameAsync("User") == null)
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
            logger.LogInformation("Создана роль User");
        }

        // Проверяем существующего администратора
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin != null)
        {
            var roles = await userManager.GetRolesAsync(existingAdmin);
            logger.LogInformation("Найден существующий администратор {Email} с ролями: {Roles}", 
                adminEmail, string.Join(", ", roles));
        }
        else
        {
            var admin = new User
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                logger.LogInformation("Создан новый администратор с email {Email}", adminEmail);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    logger.LogError("Ошибка при создании администратора: {Error}", error.Description);
                }
            }
        }
    }
} 