using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DYAT.Domain.Entities;

public class Order
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int OrderId { get; set; }
    [ForeignKey("OrderId")]
    public Order Order { get; set; }
    [Required]
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; }
    [Required]
    public int Quantity { get; set; }
    public decimal Price { get; set; }
} 