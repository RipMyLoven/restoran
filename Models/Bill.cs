namespace Restoran.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsPaid { get; set; } = false;
        public DateTime? PaidAt { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        
        public Order Order { get; set; } = null!;
    }
}
