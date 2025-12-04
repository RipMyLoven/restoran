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
    [Authorize] // Требуется авторизация
    public class MenuItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MenuItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuItems([FromQuery] int? restaurantId = null)
        {
            var query = _context.MenuItems.AsQueryable();

            if (restaurantId.HasValue)
                query = query.Where(m => m.RestaurantId == restaurantId.Value);

            var items = await query.Select(m => new MenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                Category = m.Category,
                IsAvailable = m.IsAvailable,
                RestaurantId = m.RestaurantId
            }).ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemDto>> GetMenuItem(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null)
                return NotFound();

            return Ok(new MenuItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                Category = item.Category,
                IsAvailable = item.IsAvailable,
                RestaurantId = item.RestaurantId
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem(CreateMenuItemDto dto)
        {
            // Проверяем существование ресторана
            var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == dto.RestaurantId);
            if (!restaurantExists)
                return BadRequest($"Restaurant with ID {dto.RestaurantId} does not exist");

            var item = new MenuItem
            {
                Name = dto.Name,
                Price = dto.Price,
                Category = dto.Category,
                IsAvailable = dto.IsAvailable,
                RestaurantId = dto.RestaurantId
            };

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuItem), new { id = item.Id }, new MenuItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                Category = item.Category,
                IsAvailable = item.IsAvailable,
                RestaurantId = item.RestaurantId
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenuItem(int id, CreateMenuItemDto dto)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null)
                return NotFound();

            item.Name = dto.Name;
            item.Price = dto.Price;
            item.Category = dto.Category;
            item.IsAvailable = dto.IsAvailable;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
