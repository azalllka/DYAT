using Microsoft.AspNetCore.Mvc;
using DYAT.Web.Areas.CreateNft.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DYAT.Domain.Entities;
using DYAT.Application.Interfaces.Repositories;
using AutoMapper;

namespace DYAT.Web.Areas.CreateNft.Controllers
{
    [Area("CreateNft")]
    [Authorize(Roles = "Admin")] // Только для администраторов
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly INftRepository _nftRepository;
        private readonly IMapper _mapper;

        public HomeController(
            UserManager<User> userManager,
            INftRepository nftRepository,
            IMapper mapper)
        {
            _userManager = userManager;
            _nftRepository = nftRepository;
            _mapper = mapper;
        }

        // GET
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNftViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                // Получаем текущего пользователя
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                // Проверяем, является ли пользователь администратором
                if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
                {
                    return Forbid();
                }

                // Создаем NFT
                var nft = new Nft
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    ImageUrl = await SaveImageAsync(model.ImageFile), // Метод для сохранения изображения
                    CreatedBy = currentUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // Сохраняем в базу данных
                var createdNft = await _nftRepository.CreateNftAsync(nft);

                // TODO: Здесь будет логика минтинга NFT в блокчейне

                TempData["SuccessMessage"] = "NFT успешно создан!";
                return RedirectToAction("Index", "Home", new { area = "Nfts" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Произошла ошибка при создании NFT: " + ex.Message);
                return View("Index", model);
            }
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("Файл изображения не был предоставлен");
            }

            // Проверяем расширение файла
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Недопустимый формат файла");
            }

            // Проверяем размер файла (40MB)
            if (imageFile.Length > 40 * 1024 * 1024)
            {
                throw new ArgumentException("Размер файла превышает 40MB");
            }

            // Генерируем уникальное имя файла
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine("wwwroot", "uploads", "nfts", fileName);

            // Создаем директорию, если она не существует
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Сохраняем файл
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Возвращаем относительный путь к файлу
            return $"/uploads/nfts/{fileName}";
        }
    }
}