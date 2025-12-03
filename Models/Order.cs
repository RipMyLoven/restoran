namespace Restoran.Models
{
    public enum OrderStatus
    {
        New,
        InProgress,
        Ready,
        Completed,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Table Table { get; set; } = null!;
        public Restaurant Restaurant { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Bill? Bill { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
