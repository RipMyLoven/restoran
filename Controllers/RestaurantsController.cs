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
    [Produces("application/json")]
    [Authorize] // Требуется авторизация для всех
    public class RestaurantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RestaurantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить все рестораны
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RestaurantDto>), 200)]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetRestaurants()
        {
            var restaurants = await _context.Restaurants
                .Select(r => new RestaurantDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    TableCount = r.TableCount,
                    AllergyTags = r.AllergyTags,
                    DietTags = r.DietTags
                })
                .ToListAsync();

            return Ok(restaurants);
        }

        /// <summary>
        /// Получить ресторан по ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RestaurantDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<RestaurantDto>> GetRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
                return NotFound();

            return Ok(new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                TableCount = restaurant.TableCount,
                AllergyTags = restaurant.AllergyTags,
                DietTags = restaurant.DietTags
            });
        }

        /// <summary>
        /// Создать ресторан (только Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(RestaurantDto), 201)]
        public async Task<ActionResult<RestaurantDto>> CreateRestaurant(CreateRestaurantDto dto)
        {
            var restaurant = new Restaurant
            {
                Name = dto.Name,
                TableCount = dto.TableCount,
                AllergyTags = dto.AllergyTags,
                DietTags = dto.DietTags
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            // Автоматически создаём столы
            for (int i = 1; i <= dto.TableCount; i++)
            {
                _context.Tables.Add(new Table
                {
                    Number = i,
                    Seats = 4,
                    RestaurantId = restaurant.Id
                });
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.Id },
                new RestaurantDto
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    TableCount = restaurant.TableCount,
                    AllergyTags = restaurant.AllergyTags,
                    DietTags = restaurant.DietTags
                });
        }

        /// <summary>
        /// Обновить ресторан (только Admin)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRestaurant(int id, CreateRestaurantDto dto)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
                return NotFound();

            restaurant.Name = dto.Name;
            restaurant.TableCount = dto.TableCount;
            restaurant.AllergyTags = dto.AllergyTags;
            restaurant.DietTags = dto.DietTags;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Удалить ресторан (только Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
                return NotFound();

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}