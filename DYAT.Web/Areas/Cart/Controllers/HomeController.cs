using DYAT.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DYAT.Web.Areas.Cart.Controllers;

[Area("Cart")]
[Authorize]
public class HomeController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public HomeController(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var cartItems = await _cartService.GetCartItemsAsync(userId);
        var totalPrice = await _cartService.GetTotalPriceAsync(userId);
        
        ViewBag.TotalPrice = totalPrice;
        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        await _cartService.AddToCartAsync(userId, productId, quantity);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        await _cartService.RemoveFromCartAsync(userId, productId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        await _cartService.UpdateQuantityAsync(userId, productId, quantity);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ClearCart()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        await _cartService.ClearCartAsync(userId);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckoutConfirm()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var (success, error) = await _orderService.CreateOrderAsync(userId);
        if (success)
        {
            ViewBag.Message = "Заказ успешно оформлен!";
            return View("CheckoutResult");
        }
        else
        {
            ViewBag.Error = error;
            return View("CheckoutResult");
        }
    }
}