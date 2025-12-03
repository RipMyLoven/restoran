namespace Restoran.DTOs
{
    public class BillDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class CreateBillDto
    {
        public int OrderId { get; set; }
    }
}
