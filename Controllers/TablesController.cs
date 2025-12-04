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
    public class TablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetTables([FromQuery] int? restaurantId = null)
        {
            var query = _context.Tables.AsQueryable();

            if (restaurantId.HasValue)
                query = query.Where(t => t.RestaurantId == restaurantId.Value);

            var tables = await query.Select(t => new TableDto
            {
                Id = t.Id,
                Number = t.Number,
                Seats = t.Seats,
                RestaurantId = t.RestaurantId
            }).ToListAsync();

            return Ok(tables);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TableDto>> GetTable(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
                return NotFound();

            return Ok(new TableDto
            {
                Id = table.Id,
                Number = table.Number,
                Seats = table.Seats,
                RestaurantId = table.RestaurantId
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TableDto>> CreateTable(CreateTableDto dto)
        {
            // Валидация RestaurantId
            var restaurant = await _context.Restaurants.FindAsync(dto.RestaurantId);
            if (restaurant == null)
                return BadRequest($"Restaurant with ID {dto.RestaurantId} not found. Please create a restaurant first.");

            var table = new Table
            {
                Number = dto.Number,
                Seats = dto.Seats,
                RestaurantId = dto.RestaurantId
            };

            _context.Tables.Add(table);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTable), new { id = table.Id }, new TableDto
            {
                Id = table.Id,
                Number = table.Number,
                Seats = table.Seats,
                RestaurantId = table.RestaurantId
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
                return NotFound();

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
