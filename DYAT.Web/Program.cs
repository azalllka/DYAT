using DYAT.Application.Extensions;
using DYAT.Infrastructure.Extensions;
using DYAT.Application.Interfaces.Repositories;
using DYAT.Application.Interfaces.Services;
using DYAT.Application.Services;
using DYAT.Infrastructure.Services;
using DYAT.Persistence.Extensions;
using DYAT.Persistence.Repositories;
using DYAT.Persistence.Context;
using DYAT.Web.Data;
using AutoMapper;
using Serilog;
using Microsoft.AspNetCore.Identity;
using DYAT.Domain.Entities;
using Microsoft.AspNetCore.Routing;
using DYAT.Web.Routing;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 31)
    .Enrich.WithMachineName()
    .Enrich.WithThreadId());

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddControllersWithViews();
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer();
builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавляем сервисы
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<INftRepository, NftRepository>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Добавляем настройку авторизации
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("AdminArea", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

// Инициализация ролей
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        await RoleInitializer.InitializeAsync(userManager, roleManager, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Произошла ошибка при инициализации ролей");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Важно: UseRouting должен быть перед UseAuthentication и UseAuthorization
app.UseRouting();

// Middleware для обработки ошибок
app.UseMiddleware<DYAT.Web.Middleware.ErrorHandlingMiddleware>();
app.UseExceptionHandler("/Error");
app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

// Аутентификация и авторизация
app.UseAuthentication();
app.UseAuthorization();

// Маршрутизация
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Добавляем явную настройку для админ-области
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Admin}/{action=Index}/{id?}");

app.Run();