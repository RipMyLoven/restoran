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
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("orders/{orderId}/notify/cook")]
        [Authorize(Roles = "Admin,Manager,Waiter")]
        public async Task<IActionResult> NotifyCook(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            // Find cooks in the restaurant
            var cooks = await _context.Users
                .Where(u => u.RestaurantId == order.RestaurantId && u.Role == UserRole.Cook && u.IsActive)
                .ToListAsync();

            foreach (var cook in cooks)
            {
                var notification = new Notification
                {
                    UserId = cook.Id,
                    OrderId = orderId,
                    Type = NotificationType.OrderSentToKitchen,
                    Message = $"New order #{orderId} sent to kitchen - Table {order.TableId}"
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cook notifications sent successfully" });
        }

        [HttpPost("orders/{orderId}/notify/waiter")]
        [Authorize(Roles = "Admin,Manager,Cook")]
        public async Task<IActionResult> NotifyWaiter(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.AssignedWaiter)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            // Notify assigned waiter or all waiters if no specific waiter assigned
            var waiters = new List<User>();

            if (order.AssignedWaiter != null)
            {
                waiters.Add(order.AssignedWaiter);
            }
            else
            {
                waiters = await _context.Users
                    .Where(u => u.RestaurantId == order.RestaurantId && u.Role == UserRole.Waiter && u.IsActive)
                    .ToListAsync();
            }

            foreach (var waiter in waiters)
            {
                var notification = new Notification
                {
                    UserId = waiter.Id,
                    OrderId = orderId,
                    Type = NotificationType.OrderReady,
                    Message = $"Order #{orderId} is ready - Table {order.TableId}"
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Waiter notifications sent successfully" });
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
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAllNotifications()
        {
            var notifications = await _context.Notifications
                .Include(n => n.Order)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    OrderId = n.OrderId,
                    Type = n.Type,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPost("orders/{orderId}/notify/cook")]
        [Authorize(Roles = "Admin,Manager,Waiter")]
        public async Task<IActionResult> NotifyCook(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            // Find cooks in the restaurant
            var cooks = await _context.Users
                .Where(u => u.RestaurantId == order.RestaurantId && u.Role == UserRole.Cook && u.IsActive)
                .ToListAsync();

            foreach (var cook in cooks)
            {
                var notification = new Notification
                {
                    UserId = cook.Id,
                    OrderId = orderId,
                    Type = NotificationType.OrderSentToKitchen,
                    Message = $"New order #{orderId} sent to kitchen - Table {order.TableId}"
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cook notifications sent successfully" });
        }

        [HttpPost("orders/{orderId}/notify/waiter")]
        [Authorize(Roles = "Admin,Manager,Cook")]
        public async Task<IActionResult> NotifyWaiter(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.AssignedWaiter)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            // Notify assigned waiter or all waiters if no specific waiter assigned
            var waiters = new List<User>();

            if (order.AssignedWaiter != null)
            {
                waiters.Add(order.AssignedWaiter);
            }
            else
            {
                waiters = await _context.Users
                    .Where(u => u.RestaurantId == order.RestaurantId && u.Role == UserRole.Waiter && u.IsActive)
                    .ToListAsync();
            }

            foreach (var waiter in waiters)
            {
                var notification = new Notification
                {
                    UserId = waiter.Id,
                    OrderId = orderId,
                    Type = NotificationType.OrderReady,
                    Message = $"Order #{orderId} is ready - Table {order.TableId}"
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Waiter notifications sent successfully" });
        }
    }
}
>>>>>>> 34174913b0dfb2360f689b5dc65aa2ea5b55f15d
