namespace Restoran.Models
{
    public enum UserRole
    {
        Admin,
        Manager,
        Waiter,
        Cook,
        Customer
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Customer;
        public int? RestaurantId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLoginAt { get; set; }

        public Restaurant? Restaurant { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
