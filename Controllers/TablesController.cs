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
    public class TablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TableDto>> GetTable(int id)
        {
            var table = await _context.Tables
                .Where(t => t.Id == id)
                .Select(t => new TableDto
                {
                    Id = t.Id,
                    Number = t.TableNumber,
                    Seats = t.Capacity,
                    IsAvailable = t.Status == TableStatus.Available,
                    RestaurantId = t.RestaurantId,
                    CreatedAt = DateTime.Now // Since we don't have CreatedAt in existing model
                }).FirstOrDefaultAsync();

            if (table == null)
            {
                return NotFound();
            }

            return Ok(table);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateTable(int id, UpdateTableDto updateTableDto)
        {
            var table = await _context.Tables.FindAsync(id);

            if (table == null)
            {
                return NotFound();
            }

            // Update fields if provided
            if (updateTableDto.Number.HasValue)
                table.TableNumber = updateTableDto.Number.Value;
            if (updateTableDto.Seats.HasValue)
                table.Capacity = updateTableDto.Seats.Value;
            if (updateTableDto.IsAvailable.HasValue)
                table.Status = updateTableDto.IsAvailable.Value ? TableStatus.Available : TableStatus.Occupied;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TableExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TableExists(int id)
        {
            return _context.Tables.Any(e => e.Id == id);
        }
    }

    // Add this controller for restaurant-specific table operations
    [ApiController]
    [Route("api/restaurants/{restaurantId}/tables")]
    [Authorize]
    public class RestaurantTablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RestaurantTablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetRestaurantTables(int restaurantId)
        {
            var tables = await _context.Tables
                .Where(t => t.RestaurantId == restaurantId)
                .Select(t => new TableDto
                {
                    Id = t.Id,
                    Number = t.TableNumber,
                    Seats = t.Capacity,
                    IsAvailable = t.Status == TableStatus.Available,
                    RestaurantId = t.RestaurantId,
                    CreatedAt = DateTime.Now
                }).ToListAsync();

            return Ok(tables);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<TableDto>> CreateTable(int restaurantId, CreateTableDto createTableDto)
        {
            // Check if restaurant exists
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
            {
                return BadRequest("Restaurant not found");
            }

            var table = new Table
            {
                TableNumber = createTableDto.Number,
                Capacity = createTableDto.Seats,
                RestaurantId = restaurantId,
                Status = createTableDto.IsAvailable ? TableStatus.Available : TableStatus.Occupied
            };

            _context.Tables.Add(table);
            await _context.SaveChangesAsync();

            var tableDto = new TableDto
            {
                Id = table.Id,
                Number = table.TableNumber,
                Seats = table.Capacity,
                IsAvailable = table.Status == TableStatus.Available,
                RestaurantId = table.RestaurantId,
                CreatedAt = DateTime.Now
            };

            return CreatedAtAction("GetTable", "Tables", new { id = table.Id }, tableDto);
        }
    }
}
