namespace Restoran.Models
{
    public enum TableStatus
    {
        Available,
        Occupied,
        Reserved
    }

    public class Table
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public int RestaurantId { get; set; }
        public TableStatus Status { get; set; } = TableStatus.Available;
        public int Capacity { get; set; }
        
        public Restaurant Restaurant { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
