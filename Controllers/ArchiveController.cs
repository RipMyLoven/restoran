using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Restoran.Data;
using Restoran.Models;
using Restoran.DTOs;

namespace Restoran.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize] // Требуется авторизация
    public class ArchiveController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArchiveController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить архивные заказы
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<ArchivedOrderDto>), 200)]
        public async Task<ActionResult<IEnumerable<ArchivedOrderDto>>> GetArchivedOrders(
            [FromQuery] int? restaurantId = null)
        {
            var query = _context.ArchivedOrders.AsQueryable();

            if (restaurantId.HasValue)
                query = query.Where(a => a.RestaurantId == restaurantId.Value);

            var archived = await query
                .OrderByDescending(a => a.ArchivedAt)
                .Select(a => new ArchivedOrderDto
                {
                    Id = a.Id,
                    OriginalOrderId = a.OriginalOrderId,
                    RestaurantId = a.RestaurantId,
                    TableNumber = a.TableNumber,
                    OrderItemsJson = a.OrderItemsJson,
                    Total = a.Total,
                    OrderCreatedAt = a.OrderCreatedAt,
                    ArchivedAt = a.ArchivedAt
                }).ToListAsync();

            return Ok(archived);
        }

        /// <summary>
        /// Архивировать заказ (переместить в архив)
        /// </summary>
        [HttpPost("{orderId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ArchivedOrderDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ArchivedOrderDto>> ArchiveOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound("Order not found");

            if (order.Status != OrderStatus.Completed)
                return BadRequest("Only completed orders can be archived");

            var orderItemsData = order.OrderItems.Select(oi => new
            {
                MenuItemName = oi.MenuItem.Name,
                Quantity = oi.Quantity,
                Price = oi.Price
            });

            var archived = new ArchivedOrder
            {
                OriginalOrderId = order.Id,
                RestaurantId = order.RestaurantId,
                TableNumber = order.Table.Number,
                OrderItemsJson = JsonSerializer.Serialize(orderItemsData),
                Total = order.OrderItems.Sum(oi => oi.Price * oi.Quantity),
                OrderCreatedAt = order.CreatedAt
            };

            _context.ArchivedOrders.Add(archived);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArchivedOrders), new ArchivedOrderDto
            {
                Id = archived.Id,
                OriginalOrderId = archived.OriginalOrderId,
                RestaurantId = archived.RestaurantId,
                TableNumber = archived.TableNumber,
                OrderItemsJson = archived.OrderItemsJson,
                Total = archived.Total,
                OrderCreatedAt = archived.OrderCreatedAt,
                ArchivedAt = archived.ArchivedAt
            });
        }

        /// <summary>
        /// Получить статистику по заказам
        /// </summary>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(StatisticsDto), 200)]
        public async Task<ActionResult<StatisticsDto>> GetStatistics([FromQuery] int? restaurantId = null)
        {
            var query = _context.ArchivedOrders.AsQueryable();

            if (restaurantId.HasValue)
                query = query.Where(a => a.RestaurantId == restaurantId.Value);

            var allOrders = await query.ToListAsync();
            var today = DateTime.UtcNow.Date;
            var todayOrders = allOrders.Where(o => o.ArchivedAt.Date == today).ToList();

            var stats = new StatisticsDto
            {
                TotalOrders = allOrders.Count,
                TotalRevenue = allOrders.Sum(o => o.Total),
                AverageOrderValue = allOrders.Count > 0 ? allOrders.Average(o => o.Total) : 0,
                OrdersToday = todayOrders.Count,
                RevenueToday = todayOrders.Sum(o => o.Total)
            };

            return Ok(stats);
        }
    }
}
