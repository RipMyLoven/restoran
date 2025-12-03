using System.ComponentModel.DataAnnotations;

namespace Restoran.DTOs
{
    public class TableDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Seats { get; set; }
        public bool IsAvailable { get; set; }
        public int RestaurantId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTableDto
    {
        [Required]
        [Range(1, 999)]
        public int Number { get; set; }

        [Required]
        [Range(1, 20)]
        public int Seats { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

    public class UpdateTableDto
    {
        public int? Number { get; set; }
        public int? Seats { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
