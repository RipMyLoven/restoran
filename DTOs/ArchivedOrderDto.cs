namespace Restoran.DTOs
{
    public class ArchivedOrderDto
    {
        public int Id { get; set; }
        public int OriginalOrderId { get; set; }
        public int RestaurantId { get; set; }
        public int TableNumber { get; set; }
        public string OrderItemsJson { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime OrderCreatedAt { get; set; }
        public DateTime ArchivedAt { get; set; }
    }

    public class StatisticsDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int OrdersToday { get; set; }
        public decimal RevenueToday { get; set; }
    }
}
