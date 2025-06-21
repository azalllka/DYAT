using DYAT.Application.Interfaces.Repositories;
using DYAT.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DYAT.Web.Areas.Shop.Controllers;

[Area("Shop")]
public class HomeController : Controller
{
    private readonly IProductRepository _productRepository;
    private const int PageSize = 9;

    public HomeController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IActionResult> Index(int? page, decimal? minPrice, decimal? maxPrice)
    {
        var pageNumber = page ?? 1;
        IQueryable<Domain.Entities.Product> productsQuery;

        if (minPrice.HasValue && maxPrice.HasValue)
        {
            productsQuery = await _productRepository.GetProductsByPriceRangeAsync(minPrice.Value, maxPrice.Value);
        }
        else
        {
            productsQuery = await _productRepository.GetAllProductsAsync();
        }

        var paginatedProducts = await PaginatedList<Domain.Entities.Product>.CreateAsync(
            productsQuery, pageNumber, PageSize);
            
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        return View(paginatedProducts);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    public async Task<IActionResult> Category(int id, int? page, decimal? minPrice, decimal? maxPrice)
    {
        var pageNumber = page ?? 1;
        IQueryable<Domain.Entities.Product> productsQuery;

        if (minPrice.HasValue && maxPrice.HasValue)
        {
            var categoryProducts = await _productRepository.GetProductsByCategoryAsync(id);
            productsQuery = categoryProducts.Where(p => p.Price >= minPrice.Value && p.Price <= maxPrice.Value);
        }
        else
        {
            productsQuery = await _productRepository.GetProductsByCategoryAsync(id);
        }

        var paginatedProducts = await PaginatedList<Domain.Entities.Product>.CreateAsync(
            productsQuery, pageNumber, PageSize);
            
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        return View("Index", paginatedProducts);
    }
}