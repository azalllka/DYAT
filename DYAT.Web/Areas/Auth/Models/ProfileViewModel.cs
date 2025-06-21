using System.ComponentModel.DataAnnotations;
using DYAT.Domain.Entities;

namespace DYAT.Web.Areas.Auth.Models;

public class ProfileViewModel
{
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "Wallet Balance")]
    public decimal Balance { get; set; }

    [Display(Name = "Orders")]
    public ICollection<Order> Orders { get; set; } = new List<Order>();
} 