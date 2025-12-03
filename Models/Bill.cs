namespace Restoran.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        public Order Order { get; set; } = null!;
    }
}
