namespace Restoran.Models
{
    public class ArchivedOrder
    {
        public int Id { get; set; }
        public int OriginalOrderId { get; set; }
        public int RestaurantId { get; set; }
        public int TableNumber { get; set; }
        public string OrderItemsJson { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime OrderCreatedAt { get; set; }
        public DateTime ArchivedAt { get; set; } = DateTime.UtcNow;

        public Restaurant Restaurant { get; set; } = null!;
    }
}
