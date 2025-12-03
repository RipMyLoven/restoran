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
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders([FromQuery] int? restaurantId = null, [FromQuery] string? status = null)
        {
            var query = _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .AsQueryable();

            if (restaurantId.HasValue)
            {
                query = query.Where(o => o.RestaurantId == restaurantId.Value);
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                query = query.Where(o => o.Status == orderStatus);
            }

            var orders = await query.Select(o => new OrderDto
            {
                Id = o.Id,
                TableId = o.TableId,
                RestaurantId = o.RestaurantId,
                Status = o.Status,
                SpecialRequirements = o.SpecialRequirements,
                CustomerName = o.CustomerName,
                CreatedAt = o.CreatedAt,
                SentToKitchenAt = o.SentToKitchenAt,
                ReadyAt = o.ReadyAt,
                ServedAt = o.ServedAt,
                CompletedAt = o.CompletedAt,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    Price = oi.PriceAtOrder,
                    SpecialRequirements = oi.SpecialInstructions
                }).ToList(),
                Total = o.OrderItems.Sum(oi => oi.PriceAtOrder * oi.Quantity)
            }).ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Id == id)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    TableId = o.TableId,
                    RestaurantId = o.RestaurantId,
                    Status = o.Status,
                    SpecialRequirements = o.SpecialRequirements,
                    CustomerName = o.CustomerName,
                    CreatedAt = o.CreatedAt,
                    SentToKitchenAt = o.SentToKitchenAt,
                    ReadyAt = o.ReadyAt,
                    ServedAt = o.ServedAt,
                    CompletedAt = o.CompletedAt,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        MenuItemId = oi.MenuItemId,
                        MenuItemName = oi.MenuItem.Name,
                        Quantity = oi.Quantity,
                        Price = oi.PriceAtOrder,
                        SpecialRequirements = oi.SpecialInstructions
                    }).ToList(),
                    Total = o.OrderItems.Sum(oi => oi.PriceAtOrder * oi.Quantity)
                }).FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            // Validate table exists
            var table = await _context.Tables.FindAsync(createOrderDto.TableId);
            if (table == null)
            {
                return BadRequest("Table not found");
            }

            var order = new Order
            {
                TableId = createOrderDto.TableId,
                RestaurantId = createOrderDto.RestaurantId,
                SpecialRequirements = createOrderDto.SpecialRequirements,
                CustomerName = createOrderDto.CustomerName,
                Status = OrderStatus.New
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Add order items
            foreach (var orderItemDto in createOrderDto.OrderItems)
            {
                var menuItem = await _context.MenuItems.FindAsync(orderItemDto.MenuItemId);
                if (menuItem == null)
                {
                    return BadRequest($"Menu item with ID {orderItemDto.MenuItemId} not found");
                }

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    MenuItemId = orderItemDto.MenuItemId,
                    Quantity = orderItemDto.Quantity,
                    PriceAtOrder = menuItem.Price,
                    SpecialInstructions = orderItemDto.SpecialRequirements
                };

                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();

            // Load the complete order for response
            var createdOrder = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            var orderDto = new OrderDto
            {
                Id = createdOrder!.Id,
                TableId = createdOrder.TableId,
                RestaurantId = createdOrder.RestaurantId,
                Status = createdOrder.Status,
                SpecialRequirements = createdOrder.SpecialRequirements,
                CustomerName = createdOrder.CustomerName,
                CreatedAt = createdOrder.CreatedAt,
                OrderItems = createdOrder.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    Price = oi.PriceAtOrder,
                    SpecialRequirements = oi.SpecialInstructions
                }).ToList(),
                Total = createdOrder.OrderItems.Sum(oi => oi.PriceAtOrder * oi.Quantity)
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Waiter")]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto updateOrderDto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatus.New)
            {
                return BadRequest("Can only update orders with 'New' status");
            }

            // Update basic fields
            if (updateOrderDto.SpecialRequirements != null)
                order.SpecialRequirements = updateOrderDto.SpecialRequirements;
            if (updateOrderDto.CustomerName != null)
                order.CustomerName = updateOrderDto.CustomerName;

            // Update order items if provided
            if (updateOrderDto.OrderItems != null)
            {
                // Remove existing order items
                _context.OrderItems.RemoveRange(order.OrderItems);

                // Add new order items
                foreach (var orderItemDto in updateOrderDto.OrderItems)
                {
                    var menuItem = await _context.MenuItems.FindAsync(orderItemDto.MenuItemId);
                    if (menuItem == null)
                    {
                        return BadRequest($"Menu item with ID {orderItemDto.MenuItemId} not found");
                    }

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        MenuItemId = orderItemDto.MenuItemId,
                        Quantity = orderItemDto.Quantity,
                        PriceAtOrder = menuItem.Price,
                        SpecialInstructions = orderItemDto.SpecialRequirements
                    };

                    _context.OrderItems.Add(orderItem);
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Manager,Waiter,Cook")]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto updateStatusDto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var currentStatus = order.Status;
            var newStatus = updateStatusDto.Status;

            // Validate status transitions
            if (!IsValidStatusTransition(currentStatus, newStatus))
            {
                return BadRequest($"Invalid status transition from {currentStatus} to {newStatus}");
            }

            order.Status = newStatus;

            // Update timestamp based on status
            switch (newStatus)
            {
                case OrderStatus.SentToKitchen:
                    order.SentToKitchenAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Ready:
                    order.ReadyAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Served:
                    order.ServedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Completed:
                    order.CompletedAt = DateTime.UtcNow;
                    break;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Order status updated to {newStatus}", orderId = id });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatus.New && order.Status != OrderStatus.Cancelled)
            {
                return BadRequest("Can only delete orders with 'New' or 'Cancelled' status");
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return currentStatus switch
            {
                OrderStatus.New => newStatus == OrderStatus.SentToKitchen || newStatus == OrderStatus.Cancelled,
                OrderStatus.SentToKitchen => newStatus == OrderStatus.InProgress || newStatus == OrderStatus.Cancelled,
                OrderStatus.InProgress => newStatus == OrderStatus.Ready || newStatus == OrderStatus.Cancelled,
                OrderStatus.Ready => newStatus == OrderStatus.Served || newStatus == OrderStatus.Cancelled,
                OrderStatus.Served => newStatus == OrderStatus.Completed,
                _ => false
            };
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}