using System.ComponentModel.DataAnnotations;

namespace Restoran.DTOs
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TableCount { get; set; }
        public string AllergyTags { get; set; } = string.Empty;
        public string DietTags { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRestaurantDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 500)]
        public int TableCount { get; set; }

        public string AllergyTags { get; set; } = string.Empty;
        public string DietTags { get; set; } = string.Empty;
    }

    public class UpdateRestaurantDto
    {
        public string? Name { get; set; }
        public int? TableCount { get; set; }
        public string? AllergyTags { get; set; }
        public string? DietTags { get; set; }
    }
}
