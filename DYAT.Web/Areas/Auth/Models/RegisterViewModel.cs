using System.ComponentModel.DataAnnotations;

namespace DYAT.Web.Areas.Auth.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите имя пользователя")]
    [StringLength(100, ErrorMessage = "Имя пользователя должно содержать от {2} до {1} символов.", MinimumLength = 2)]
    [Display(Name = "Имя пользователя")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [StringLength(100, ErrorMessage = "Пароль должен содержать минимум {2} символов.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение пароля")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmPassword { get; set; } = string.Empty;
} 