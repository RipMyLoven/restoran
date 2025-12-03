namespace Restoran.Models
{
    public enum UserRole
    {
        Admin,
        Waiter,
        Cook
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Waiter;
        public int? RestaurantId { get; set; }

        public Restaurant? Restaurant { get; set; }
    }
}
