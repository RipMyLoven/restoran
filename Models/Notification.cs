namespace Restoran.Models
{
    public enum NotificationType
    {
        OrderCreated,
        OrderReady,
        OrderServed
    }

    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Order Order { get; set; } = null!;
    }
}
