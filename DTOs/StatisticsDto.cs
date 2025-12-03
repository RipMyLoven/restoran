namespace Restoran.DTOs
{
    public class StatisticsDto
    {
        public class OrderStatistics
        {
            public int TotalOrders { get; set; }
            public int CompletedOrders { get; set; }
            public int CancelledOrders { get; set; }
            public decimal TotalRevenue { get; set; }
            public decimal AverageOrderValue { get; set; }
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
        }

        public class KitchenStatistics
        {
            public int PendingOrders { get; set; }
            public int InProgressOrders { get; set; }
            public int ReadyOrders { get; set; }
            public double AveragePreparationTimeMinutes { get; set; }
            public List<PopularMenuItem> PopularItems { get; set; } = new List<PopularMenuItem>();
        }

        public class WaiterStatistics
        {
            public int AssignedTables { get; set; }
            public int ActiveOrders { get; set; }
            public int ServedOrders { get; set; }
            public double AverageServiceTimeMinutes { get; set; }
        }

        public class PopularMenuItem
        {
            public int MenuItemId { get; set; }
            public string MenuItemName { get; set; } = string.Empty;
            public int OrderCount { get; set; }
            public int TotalQuantity { get; set; }
        }
    }
}
