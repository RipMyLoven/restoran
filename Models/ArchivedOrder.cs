namespace Restoran.Models
{
    public class ArchivedOrder
    {
        public int Id { get; set; }
        public int OriginalOrderId { get; set; }
        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SpecialRequirements { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? SentToKitchenAt { get; set; }
        public DateTime? ReadyAt { get; set; }
        public DateTime? ServedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime ArchivedAt { get; set; } = DateTime.Now;
        public string OrderItemsJson { get; set; } = string.Empty; // Serialized OrderItems
        public decimal Total { get; set; }

        public Table Table { get; set; } = null!;
        public Restaurant Restaurant { get; set; } = null!;
    }
}
