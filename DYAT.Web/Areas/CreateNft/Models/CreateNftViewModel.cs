using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DYAT.Web.Areas.CreateNft.Models
{
    public class CreateNftViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, загрузите изображение")]
        [Display(Name = "Изображение NFT")]
        public IFormFile ImageFile { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название NFT")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Название должно содержать от 3 до 100 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите описание")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Описание должно содержать от 10 до 1000 символов")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Пожалуйста, введите корректный URL")]
        [Display(Name = "Внешняя ссылка")]
        public string ExternalLink { get; set; }

        [Display(Name = "Трейты")]
        public string Traits { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите цену")]
        [Range(0.000001, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }
    }
} 