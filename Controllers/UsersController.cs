using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restoran.Data;
using Restoran.Models;
using Restoran.DTOs;

namespace Restoran.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Только Admin может управлять пользователями
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                RestaurantId = u.RestaurantId
            }).ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                RestaurantId = user.RestaurantId
            });
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                RestaurantId = dto.RestaurantId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                RestaurantId = user.RestaurantId
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
