<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restoran.Data;
using Restoran.Models;
using Restoran.DTOs;

namespace Restoran.Controllers
{
    [ApiController]
    [Route("api/menu")]
    [Authorize]
    public class MenuItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MenuItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemDto>> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems
                .Where(m => m.Id == id)
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    Category = m.Category,
                    AllergyInfo = m.Allergens,
                    DietInfo = m.DietaryInfo,
                    IsAvailable = m.IsAvailable,
                    RestaurantId = m.RestaurantId,
                    CreatedAt = DateTime.Now // Since we don't have CreatedAt in existing model
                }).FirstOrDefaultAsync();

            if (menuItem == null)
            {
                return NotFound();
            }

            return Ok(menuItem);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateMenuItem(int id, UpdateMenuItemDto updateMenuItemDto)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);

            if (menuItem == null)
            {
                return NotFound();
            }

            // Update fields if provided
            if (updateMenuItemDto.Name != null)
                menuItem.Name = updateMenuItemDto.Name;
            if (updateMenuItemDto.Description != null)
                menuItem.Description = updateMenuItemDto.Description;
            if (updateMenuItemDto.Price.HasValue)
                menuItem.Price = updateMenuItemDto.Price.Value;
            if (updateMenuItemDto.Category != null)
                menuItem.Category = updateMenuItemDto.Category;
            if (updateMenuItemDto.AllergyInfo != null)
                menuItem.Allergens = updateMenuItemDto.AllergyInfo;
            if (updateMenuItemDto.DietInfo != null)
                menuItem.DietaryInfo = updateMenuItemDto.DietInfo;
            if (updateMenuItemDto.IsAvailable.HasValue)
                menuItem.IsAvailable = updateMenuItemDto.IsAvailable.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuItemExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.Id == id);
        }
    }

    // Add this controller for restaurant-specific menu operations
    [ApiController]
    [Route("api/restaurants/{restaurantId}/menu")]
    [Authorize]
    public class RestaurantMenuController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RestaurantMenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetRestaurantMenu(int restaurantId)
        {
            var menuItems = await _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId)
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    Category = m.Category,
                    AllergyInfo = m.Allergens,
                    DietInfo = m.DietaryInfo,
                    IsAvailable = m.IsAvailable,
                    RestaurantId = m.RestaurantId,
                    CreatedAt = DateTime.Now
                }).ToListAsync();

            return Ok(menuItems);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem(int restaurantId, CreateMenuItemDto createMenuItemDto)
        {
            // Check if restaurant exists
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
            {
                return BadRequest("Restaurant not found");
            }

            var menuItem = new MenuItem
            {
                Name = createMenuItemDto.Name,
                Description = createMenuItemDto.Description,
                Price = createMenuItemDto.Price,
                Category = createMenuItemDto.Category,
                Allergens = createMenuItemDto.AllergyInfo,
                DietaryInfo = createMenuItemDto.DietInfo,
                RestaurantId = restaurantId,
                IsAvailable = createMenuItemDto.IsAvailable
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var menuItemDto = new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                Category = menuItem.Category,
                AllergyInfo = menuItem.Allergens,
                DietInfo = menuItem.DietaryInfo,
                IsAvailable = menuItem.IsAvailable,
                RestaurantId = menuItem.RestaurantId,
                CreatedAt = DateTime.Now
            };

            return CreatedAtAction("GetMenuItem", "MenuItems", new { id = menuItem.Id }, menuItemDto);
        }
    }
}
=======
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restoran.Data;
using Restoran.Models;
using Restoran.DTOs;

namespace Restoran.Controllers
{
    [ApiController]
    [Route("api/menu")]
    [Authorize]
    public class MenuItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MenuItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetAllMenuItems()
        {
            var menuItems = await _context.MenuItems
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    Category = m.Category,
                    AllergyInfo = m.Allergens,
                    DietInfo = m.DietaryInfo,
                    IsAvailable = m.IsAvailable,
                    RestaurantId = m.RestaurantId,
                    CreatedAt = DateTime.Now
                }).ToListAsync();

            return Ok(menuItems);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemDto>> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems
                .Where(m => m.Id == id)
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    Category = m.Category,
                    AllergyInfo = m.Allergens,
                    DietInfo = m.DietaryInfo,
                    IsAvailable = m.IsAvailable,
                    RestaurantId = m.RestaurantId,
                    CreatedAt = DateTime.Now // Since we don't have CreatedAt in existing model
                }).FirstOrDefaultAsync();

            if (menuItem == null)
            {
                return NotFound();
            }

            return Ok(menuItem);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Waiter")]
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem(CreateMenuItemDto createMenuItemDto)
        {
            var menuItem = new MenuItem
            {
                Name = createMenuItemDto.Name,
                Description = createMenuItemDto.Description,
                Price = createMenuItemDto.Price,
                Category = createMenuItemDto.Category,
                Allergens = createMenuItemDto.AllergyInfo,
                DietaryInfo = createMenuItemDto.DietInfo,
                IsAvailable = createMenuItemDto.IsAvailable,
                RestaurantId = createMenuItemDto.RestaurantId
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var menuItemDto = new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                Category = menuItem.Category,
                AllergyInfo = menuItem.Allergens,
                DietInfo = menuItem.DietaryInfo,
                IsAvailable = menuItem.IsAvailable,
                RestaurantId = menuItem.RestaurantId,
                CreatedAt = DateTime.Now
            };

            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.Id }, menuItemDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateMenuItem(int id, UpdateMenuItemDto updateMenuItemDto)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);

            if (menuItem == null)
            {
                return NotFound();
            }

            // Update fields if provided
            if (updateMenuItemDto.Name != null)
                menuItem.Name = updateMenuItemDto.Name;
            if (updateMenuItemDto.Description != null)
                menuItem.Description = updateMenuItemDto.Description;
            if (updateMenuItemDto.Price.HasValue)
                menuItem.Price = updateMenuItemDto.Price.Value;
            if (updateMenuItemDto.Category != null)
                menuItem.Category = updateMenuItemDto.Category;
            if (updateMenuItemDto.AllergyInfo != null)
                menuItem.Allergens = updateMenuItemDto.AllergyInfo;
            if (updateMenuItemDto.DietInfo != null)
                menuItem.DietaryInfo = updateMenuItemDto.DietInfo;
            if (updateMenuItemDto.IsAvailable.HasValue)
                menuItem.IsAvailable = updateMenuItemDto.IsAvailable.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuItemExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.Id == id);
        }
    }

    // Add this controller for restaurant-specific menu operations
    [ApiController]
    [Route("api/restaurants/{restaurantId}/menu")]
    [Authorize]
    public class RestaurantMenuController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RestaurantMenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetRestaurantMenu([FromRoute] int restaurantId)
        {
            var menuItems = await _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId)
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    Category = m.Category,
                    AllergyInfo = m.Allergens,
                    DietInfo = m.DietaryInfo,
                    IsAvailable = m.IsAvailable,
                    RestaurantId = m.RestaurantId,
                    CreatedAt = DateTime.Now
                }).ToListAsync();

            return Ok(menuItems);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem([FromRoute] int restaurantId, CreateMenuItemDto createMenuItemDto)
        {
            // Check if restaurant exists
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
            {
                return BadRequest("Restaurant not found");
            }

            var menuItem = new MenuItem
            {
                Name = createMenuItemDto.Name,
                Description = createMenuItemDto.Description,
                Price = createMenuItemDto.Price,
                Category = createMenuItemDto.Category,
                Allergens = createMenuItemDto.AllergyInfo,
                DietaryInfo = createMenuItemDto.DietInfo,
                RestaurantId = restaurantId,
                IsAvailable = createMenuItemDto.IsAvailable
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var menuItemDto = new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                Category = menuItem.Category,
                AllergyInfo = menuItem.Allergens,
                DietInfo = menuItem.DietaryInfo,
                IsAvailable = menuItem.IsAvailable,
                RestaurantId = menuItem.RestaurantId,
                CreatedAt = DateTime.Now
            };

            return CreatedAtAction("GetMenuItem", "MenuItems", new { id = menuItem.Id }, menuItemDto);
        }
    }
}
>>>>>>> 34174913b0dfb2360f689b5dc65aa2ea5b55f15d
