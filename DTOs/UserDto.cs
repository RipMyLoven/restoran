using Restoran.Models;

namespace Restoran.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public int? RestaurantId { get; set; }
    }

    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Waiter;
        public int? RestaurantId { get; set; }
    }
}
