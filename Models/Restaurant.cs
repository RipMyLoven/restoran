namespace Restoran.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TableCount { get; set; }
        public string AllergyTags { get; set; } = string.Empty;
        public string DietTags { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Table> Tables { get; set; } = new List<Table>();
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<ArchivedOrder> ArchivedOrders { get; set; } = new List<ArchivedOrder>();
    }
}
