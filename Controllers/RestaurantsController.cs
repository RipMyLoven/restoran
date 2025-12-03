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
    public class RestaurantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RestaurantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetRestaurants()
        {
            var restaurants = await _context.Restaurants
                .Select(r => new RestaurantDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    TableCount = r.TableCount,
                    AllergyTags = r.AllergyTags,
                    DietTags = r.DietTags,
                    CreatedAt = r.CreatedAt
                }).ToListAsync();

            return Ok(restaurants);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantDto>> GetRestaurant(int id)
        {
            var restaurant = await _context.Restaurants
                .Where(r => r.Id == id)
                .Select(r => new RestaurantDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    TableCount = r.TableCount,
                    AllergyTags = r.AllergyTags,
                    DietTags = r.DietTags,
                    CreatedAt = r.CreatedAt
                }).FirstOrDefaultAsync();

            if (restaurant == null)
            {
                return NotFound();
            }

            return Ok(restaurant);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<RestaurantDto>> CreateRestaurant(CreateRestaurantDto createRestaurantDto)
        {
            var restaurant = new Restaurant
            {
                Name = createRestaurantDto.Name,
                TableCount = createRestaurantDto.TableCount,
                AllergyTags = createRestaurantDto.AllergyTags,
                DietTags = createRestaurantDto.DietTags
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            // Create tables for the restaurant
            for (int i = 1; i <= restaurant.TableCount; i++)
            {
                _context.Tables.Add(new Table
                {
                    TableNumber = i,
                    RestaurantId = restaurant.Id,
                    Capacity = 4,
                    Status = TableStatus.Available
                });
            }
            await _context.SaveChangesAsync();

            var restaurantDto = new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                TableCount = restaurant.TableCount,
                AllergyTags = restaurant.AllergyTags,
                DietTags = restaurant.DietTags,
                CreatedAt = restaurant.CreatedAt
            };

            return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.Id }, restaurantDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateRestaurant(int id, UpdateRestaurantDto updateRestaurantDto)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            // Update fields if provided
            if (updateRestaurantDto.Name != null)
                restaurant.Name = updateRestaurantDto.Name;
            if (updateRestaurantDto.TableCount.HasValue)
                restaurant.TableCount = updateRestaurantDto.TableCount.Value;
            if (updateRestaurantDto.AllergyTags != null)
                restaurant.AllergyTags = updateRestaurantDto.AllergyTags;
            if (updateRestaurantDto.DietTags != null)
                restaurant.DietTags = updateRestaurantDto.DietTags;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RestaurantExists(int id)
        {
            return _context.Restaurants.Any(e => e.Id == id);
        }
    }
}