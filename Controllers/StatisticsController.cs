using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restoran.Data;
using Restoran.Models;
using Restoran.DTOs;
using static Restoran.DTOs.StatisticsDto;

namespace Restoran.Controllers
{
    [ApiController]
    [Route("api/stats")]
    [Authorize(Roles = "Admin,Manager")]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("orders")]
        public async Task<ActionResult<OrderStatistics>> GetOrderStatistics(
            [FromQuery] int? restaurantId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var query = _context.Orders.AsQueryable();

            if (restaurantId.HasValue)
            {
                query = query.Where(o => o.RestaurantId == restaurantId.Value);
            }

            if (from.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= to.Value);
            }

            var orders = await query
                .Include(o => o.OrderItems)
                .ToListAsync();

            var completedOrders = orders.Where(o => o.Status == OrderStatus.Completed).ToList();
            var totalRevenue = completedOrders.Sum(o => o.OrderItems.Sum(oi => oi.PriceAtOrder * oi.Quantity));

            var stats = new OrderStatistics
            {
                TotalOrders = orders.Count,
                CompletedOrders = completedOrders.Count,
                CancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled),
                TotalRevenue = totalRevenue,
                AverageOrderValue = completedOrders.Count > 0
                    ? completedOrders.Average(o => o.OrderItems.Sum(oi => oi.PriceAtOrder * oi.Quantity))
                    : 0,
                From = from,
                To = to
            };

            return Ok(stats);
        }

        [HttpGet("kitchen")]
        public async Task<ActionResult<KitchenStatistics>> GetKitchenStatistics([FromQuery] int? restaurantId = null)
        {
            var query = _context.Orders.AsQueryable();

            if (restaurantId.HasValue)
            {
                query = query.Where(o => o.RestaurantId == restaurantId.Value);
            }

            var orders = await query.ToListAsync();

            var completedOrdersWithTimes = orders
                .Where(o => o.Status == OrderStatus.Completed &&
                           o.SentToKitchenAt.HasValue &&
                           o.ReadyAt.HasValue)
                .ToList();

            var avgPrepTime = completedOrdersWithTimes.Any()
                ? completedOrdersWithTimes.Average(o => (o.ReadyAt!.Value - o.SentToKitchenAt!.Value).TotalMinutes)
                : 0;

            var popularItems = await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Where(oi => restaurantId == null || oi.MenuItem.RestaurantId == restaurantId)
                .GroupBy(oi => new { oi.MenuItemId, oi.MenuItem.Name })
                .Select(g => new PopularMenuItem
                {
                    MenuItemId = g.Key.MenuItemId,
                    MenuItemName = g.Key.Name,
                    OrderCount = g.Count(),
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToListAsync();

            var stats = new KitchenStatistics
            {
                PendingOrders = orders.Count(o => o.Status == OrderStatus.SentToKitchen),
                InProgressOrders = orders.Count(o => o.Status == OrderStatus.InProgress),
                ReadyOrders = orders.Count(o => o.Status == OrderStatus.Ready),
                AveragePreparationTimeMinutes = Math.Round(avgPrepTime, 2),
                PopularItems = popularItems
            };

            return Ok(stats);
        }

        [HttpGet("waiters")]
        public async Task<ActionResult<WaiterStatistics>> GetWaiterStatistics([FromQuery] int? userId = null, [FromQuery] int? restaurantId = null)
        {
            var query = _context.Orders.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(o => o.AssignedWaiterId == userId.Value);
            }

            if (restaurantId.HasValue)
            {
                query = query.Where(o => o.RestaurantId == restaurantId.Value);
            }

            var orders = await query.ToListAsync();

            var servedOrdersWithTimes = orders
                .Where(o => o.Status == OrderStatus.Served &&
                           o.ReadyAt.HasValue &&
                           o.ServedAt.HasValue)
                .ToList();

            var avgServiceTime = servedOrdersWithTimes.Any()
                ? servedOrdersWithTimes.Average(o => (o.ServedAt!.Value - o.ReadyAt!.Value).TotalMinutes)
                : 0;

            var assignedTableIds = orders.Select(o => o.TableId).Distinct().ToList();

            var stats = new WaiterStatistics
            {
                AssignedTables = assignedTableIds.Count,
                ActiveOrders = orders.Count(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled),
                ServedOrders = orders.Count(o => o.Status == OrderStatus.Served || o.Status == OrderStatus.Completed),
                AverageServiceTimeMinutes = Math.Round(avgServiceTime, 2)
            };

            return Ok(stats);
        }
    }
}