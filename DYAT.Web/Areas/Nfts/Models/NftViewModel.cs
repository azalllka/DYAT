using System.ComponentModel.DataAnnotations;

namespace DYAT.Web.Areas.Nfts.Models;

public class NftViewModel
{
    public int Id { get; set; }

    [Display(Name = "Название")]
    public string Name { get; set; }

    [Display(Name = "Описание")]
    public string Description { get; set; }

    [Display(Name = "Изображение")]
    public string ImageUrl { get; set; }

    [Display(Name = "Цена")]
    public decimal Price { get; set; }

    [Display(Name = "Создатель")]
    public string Creator { get; set; }

    [Display(Name = "Дата создания")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Высшая ставка")]
    public decimal HighestBid { get; set; }

    [Display(Name = "Рейтинг")]
    public int Rating { get; set; }
} 