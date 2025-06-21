using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DYAT.Application.Interfaces.Repositories;
using DYAT.Web.Areas.Nfts.Models;
using Microsoft.AspNetCore.Identity;
using DYAT.Domain.Entities;

namespace DYAT.Web.Areas.Nfts.Controllers
{
    [Area("Nfts")]
    public class HomeController : Controller
    {
        private readonly INftRepository _nftRepository;
        private readonly UserManager<User> _userManager;

        public HomeController(INftRepository nftRepository, UserManager<User> userManager)
        {
            _nftRepository = nftRepository;
            _userManager = userManager;
        }

        // Вывод только свободных NFT
        public async Task<IActionResult> Index()
        {
            var nfts = await _nftRepository.GetAllNftsAsync();
            var freeNfts = nfts.Where(n => n.OwnerId == null && n.IsActive).ToList();
            var viewModels = freeNfts.Select(n => new NftViewModel
            {
                Id = n.Id,
                Name = n.Name,
                Description = n.Description,
                ImageUrl = n.ImageUrl,
                Price = n.Price,
                Creator = n.Creator?.UserName ?? "",
                CreatedAt = n.CreatedAt,
                HighestBid = n.Price,
                Rating = 5
            }).ToList();
            return View(viewModels);
        }

        // Покупка NFT
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Buy(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var nft = (await _nftRepository.GetAllNftsAsync()).FirstOrDefault(n => n.Id == id);
            if (nft == null || nft.OwnerId != null)
            {
                return NotFound();
            }
            nft.OwnerId = user.Id;
            await _nftRepository.UpdateNftAsync(nft);
            return RedirectToAction("MyNfts");
        }

        // NFT пользователя
        [Authorize]
        public async Task<IActionResult> MyNfts()
        {
            var user = await _userManager.GetUserAsync(User);
            var nfts = await _nftRepository.GetAllNftsAsync();
            var myNfts = nfts.Where(n => n.OwnerId == user.Id).ToList();
            var viewModels = myNfts.Select(n => new NftViewModel
            {
                Id = n.Id,
                Name = n.Name,
                Description = n.Description,
                ImageUrl = n.ImageUrl,
                Price = n.Price,
                Creator = n.Creator?.UserName ?? "",
                CreatedAt = n.CreatedAt,
                HighestBid = n.Price,
                Rating = 5
            }).ToList();
            return View(viewModels);
        }

        // Удаление NFT (только для администратора)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _nftRepository.DeleteNftAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "NFT успешно удален";
            return RedirectToAction("Index");
        }
    }
}