namespace Restoran.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Allergens { get; set; } = string.Empty;
        public string DietaryInfo { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public bool IsAvailable { get; set; } = true;
        
        public Restaurant Restaurant { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
