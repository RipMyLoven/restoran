using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restoran.Data;
using Restoran.Models;
using Restoran.DTOs;

namespace Restoran.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] int? restaurantId = null)
        {
            var query = _context.Users.AsQueryable();

            if (restaurantId.HasValue)
            {
                query = query.Where(u => u.RestaurantId == restaurantId.Value);
            }

            var users = await query.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                RestaurantId = u.RestaurantId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            }).ToListAsync();

            return Ok(users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Username == createUserDto.Username || u.Email == createUserDto.Email))
            {
                return BadRequest("User with this username or email already exists");
            }

            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                PhoneNumber = createUserDto.PhoneNumber,
                Role = createUserDto.Role,
                RestaurantId = createUserDto.RestaurantId,
                IsActive = createUserDto.IsActive
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                RestaurantId = user.RestaurantId,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Check authorization
            var currentUserId = GetCurrentUserId();
            if (currentUserId != id && !IsAdminOrManager())
            {
                return Forbid();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                RestaurantId = user.RestaurantId,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Check authorization
            var currentUserId = GetCurrentUserId();
            if (currentUserId != id && !IsAdminOrManager())
            {
                return Forbid();
            }

            // Update fields if provided
            if (updateUserDto.Username != null)
                user.Username = updateUserDto.Username;
            if (updateUserDto.Email != null)
                user.Email = updateUserDto.Email;
            if (updateUserDto.FirstName != null)
                user.FirstName = updateUserDto.FirstName;
            if (updateUserDto.LastName != null)
                user.LastName = updateUserDto.LastName;
            if (updateUserDto.PhoneNumber != null)
                user.PhoneNumber = updateUserDto.PhoneNumber;

            // Only admins/managers can change role, restaurant, and active status
            if (IsAdminOrManager())
            {
                if (updateUserDto.Role.HasValue)
                    user.Role = updateUserDto.Role.Value;
                if (updateUserDto.RestaurantId.HasValue)
                    user.RestaurantId = updateUserDto.RestaurantId.Value;
                if (updateUserDto.IsActive.HasValue)
                    user.IsActive = updateUserDto.IsActive.Value;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        private bool IsAdminOrManager()
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return role == "Admin" || role == "Manager";
        }
    }
}
