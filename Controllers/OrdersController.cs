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
    [Authorize] // Требуется авторизация
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить все заказы
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders([FromQuery] int? restaurantId = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .AsQueryable();

            if (restaurantId.HasValue)
                query = query.Where(o => o.RestaurantId == restaurantId.Value);

            var orders = await query.Select(o => new OrderDto
            {
                Id = o.Id,
                TableId = o.TableId,
                RestaurantId = o.RestaurantId,
                Status = o.Status,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList(),
                Total = o.OrderItems.Sum(oi => oi.Price * oi.Quantity)
            }).ToListAsync();

            return Ok(orders);
        }

        /// <summary>
        /// Получить заказ по ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Id == id)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    TableId = o.TableId,
                    RestaurantId = o.RestaurantId,
                    Status = o.Status,
                    Notes = o.Notes,
                    CreatedAt = o.CreatedAt,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        MenuItemId = oi.MenuItemId,
                        MenuItemName = oi.MenuItem.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList(),
                    Total = o.OrderItems.Sum(oi => oi.Price * oi.Quantity)
                }).FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        /// <summary>
        /// Создать новый заказ (только Admin, Waiter)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Waiter")]
        [ProducesResponseType(typeof(OrderDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto dto)
        {
            var restaurant = await _context.Restaurants.FindAsync(dto.RestaurantId);
            if (restaurant == null)
                return BadRequest($"Restaurant with ID {dto.RestaurantId} not found. Please create a restaurant first.");

            var table = await _context.Tables.FindAsync(dto.TableId);
            if (table == null)
                return BadRequest($"Table with ID {dto.TableId} not found. Please create a table first.");

            var order = new Order
            {
                TableId = dto.TableId,
                RestaurantId = dto.RestaurantId,
                Notes = dto.Notes,
                Status = OrderStatus.New
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in dto.OrderItems)
            {
                var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
                if (menuItem == null)
                    return BadRequest($"MenuItem {item.MenuItemId} not found");

                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    Price = menuItem.Price
                });
            }

            var cooks = await _context.Users
                .Where(u => u.Role == UserRole.Cook && u.RestaurantId == dto.RestaurantId)
                .ToListAsync();

            foreach (var cook in cooks)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = cook.Id,
                    OrderId = order.Id,
                    Type = NotificationType.OrderCreated,
                    Message = $"Новый заказ #{order.Id} для стола {dto.TableId}"
                });
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, null);
        }

        /// <summary>
        /// Обновить статус заказа (Admin, Cook, Waiter)
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Cook,Waiter")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            var oldStatus = order.Status;
            order.Status = dto.Status;

            if (dto.Status == OrderStatus.Ready && oldStatus != OrderStatus.Ready)
            {
                var waiters = await _context.Users
                    .Where(u => u.Role == UserRole.Waiter && u.RestaurantId == order.RestaurantId)
                    .ToListAsync();

                foreach (var waiter in waiters)
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = waiter.Id,
                        OrderId = order.Id,
                        Type = NotificationType.OrderReady,
                        Message = $"Заказ #{order.Id} готов к подаче"
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { order.Id, order.Status });
        }

        /// <summary>
        /// Удалить заказ (только Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}