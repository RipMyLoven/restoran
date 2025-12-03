namespace Restoran.Models
{
    public enum OrderStatus
    {
        New,
        SentToKitchen,
        InProgress,
        Ready,
        Served,
        Completed,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public int? AssignedWaiterId { get; set; }
        public int? AssignedCookId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public string SpecialRequirements { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? SentToKitchenAt { get; set; }
        public DateTime? ReadyAt { get; set; }
        public DateTime? ServedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Table Table { get; set; } = null!;
        public Restaurant Restaurant { get; set; } = null!;
        public User? AssignedWaiter { get; set; }
        public User? AssignedCook { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Bill? Bill { get; set; }
        public Invoice? Invoice { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
