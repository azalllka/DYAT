using System.ComponentModel.DataAnnotations;

namespace DYAT.Domain.Entities
{
    public class Nft
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; }

        [Required]
        [Range(0.000001, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public string? OwnerId { get; set; }
        public virtual User? Owner { get; set; }

        // Навигационные свойства
        public virtual User Creator { get; set; }
    }
} 