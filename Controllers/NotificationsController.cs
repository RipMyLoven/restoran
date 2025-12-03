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
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить уведомления для пользователя
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificationDto>), 200)]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications(
            [FromQuery] int? userId = null,
            [FromQuery] bool? unreadOnly = null)
        {
            var query = _context.Notifications.AsQueryable();

            if (userId.HasValue)
                query = query.Where(n => n.UserId == userId.Value);

            if (unreadOnly == true)
                query = query.Where(n => !n.IsRead);

            var notifications = await query
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
                }).ToListAsync();

            return Ok(notifications);
        }

        /// <summary>
        /// Отметить уведомление как прочитанное
        /// </summary>
        [HttpPatch("{id}/read")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Marked as read" });
        }

        /// <summary>
        /// Отметить все уведомления пользователя как прочитанные
        /// </summary>
        [HttpPatch("read-all")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> MarkAllAsRead([FromQuery] int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifications)
                n.IsRead = true;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Marked {notifications.Count} notifications as read" });
        }
    }
}
