using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restoran.Data;
using Restoran.Models;
using Restoran.DTOs;
using System.Text.Json;

namespace Restoran.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ArchiveController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArchiveController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("orders/{orderId}/archive")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ArchiveOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            if (order.Status != OrderStatus.Completed && order.Status != OrderStatus.Cancelled)
            {
                return BadRequest("Only completed or cancelled orders can be archived");
            }

            // Serialize order items
            var orderItemsData = order.OrderItems.Select(oi => new
            {
                MenuItemName = oi.MenuItem.Name,
                Quantity = oi.Quantity,
                Price = oi.PriceAtOrder,
                SpecialInstructions = oi.SpecialInstructions
            }).ToList();

            var archivedOrder = new ArchivedOrder
            {
                OriginalOrderId = order.Id,
                TableId = order.TableId,
                RestaurantId = order.RestaurantId,
                Status = order.Status.ToString(),
                SpecialRequirements = order.SpecialRequirements,
                CustomerName = order.CustomerName,
                CreatedAt = order.CreatedAt,
                SentToKitchenAt = order.SentToKitchenAt,
                ReadyAt = order.ReadyAt,
                ServedAt = order.ServedAt,
                CompletedAt = order.CompletedAt,
                OrderItemsJson = JsonSerializer.Serialize(orderItemsData),
                Total = order.OrderItems.Sum(oi => oi.PriceAtOrder * oi.Quantity)
            };

            _context.Set<ArchivedOrder>().Add(archivedOrder);

            // Remove original order and related data
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Order archived successfully", archivedOrderId = archivedOrder.Id });
        }

        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<object>>> GetArchivedOrders(
            [FromQuery] int? restaurantId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = _context.Set<ArchivedOrder>().AsQueryable();

            if (restaurantId.HasValue)
            {
                query = query.Where(ao => ao.RestaurantId == restaurantId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(ao => ao.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(ao => ao.CreatedAt <= toDate.Value);
            }

            var archivedOrders = await query
                .OrderByDescending(ao => ao.ArchivedAt)
                .ToListAsync();

            var result = archivedOrders.Select(ao => new
            {
                Id = ao.Id,
                OriginalOrderId = ao.OriginalOrderId,
                TableId = ao.TableId,
                RestaurantId = ao.RestaurantId,
                Status = ao.Status,
                SpecialRequirements = ao.SpecialRequirements,
                CustomerName = ao.CustomerName,
                CreatedAt = ao.CreatedAt,
                SentToKitchenAt = ao.SentToKitchenAt,
                ReadyAt = ao.ReadyAt,
                ServedAt = ao.ServedAt,
                CompletedAt = ao.CompletedAt,
                ArchivedAt = ao.ArchivedAt,
                Total = ao.Total,
                OrderItems = JsonSerializer.Deserialize<List<object>>(ao.OrderItemsJson)
            }).ToList();

            return Ok(result);
        }
    }
}
