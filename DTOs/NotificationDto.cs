using Restoran.Models;

namespace Restoran.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
