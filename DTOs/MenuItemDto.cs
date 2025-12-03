<<<<<<< HEAD
using System.ComponentModel.DataAnnotations;

namespace Restoran.DTOs
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string AllergyInfo { get; set; } = string.Empty;
        public string DietInfo { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int RestaurantId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMenuItemDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999.99)]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        public string AllergyInfo { get; set; } = string.Empty;
        public string DietInfo { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
    }

    public class UpdateMenuItemDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Category { get; set; }
        public string? AllergyInfo { get; set; }
        public string? DietInfo { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
=======
using System.ComponentModel.DataAnnotations;

namespace Restoran.DTOs
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string AllergyInfo { get; set; } = string.Empty;
        public string DietInfo { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int RestaurantId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMenuItemDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999.99)]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        public string AllergyInfo { get; set; } = string.Empty;
        public string DietInfo { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public int RestaurantId { get; set; } = 1; // Default to 1
    }

    public class UpdateMenuItemDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Category { get; set; }
        public string? AllergyInfo { get; set; }
        public string? DietInfo { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
>>>>>>> 34174913b0dfb2360f689b5dc65aa2ea5b55f15d
