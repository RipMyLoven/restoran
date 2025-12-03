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
    [Authorize(Roles = "Admin,Waiter")] // Только Admin и Waiter работают со счетами
    public class BillsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить все счета
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BillDto>), 200)]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetBills([FromQuery] int? orderId = null)
        {
            var query = _context.Bills.AsQueryable();

            if (orderId.HasValue)
                query = query.Where(b => b.OrderId == orderId.Value);

            var bills = await query.Select(b => new BillDto
            {
                Id = b.Id,
                OrderId = b.OrderId,
                Total = b.Total,
                IsPaid = b.IsPaid,
                CreatedAt = b.CreatedAt,
                PaidAt = b.PaidAt
            }).ToListAsync();

            return Ok(bills);
        }

        /// <summary>
        /// Получить счёт по ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BillDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BillDto>> GetBill(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
                return NotFound();

            return Ok(new BillDto
            {
                Id = bill.Id,
                OrderId = bill.OrderId,
                Total = bill.Total,
                IsPaid = bill.IsPaid,
                CreatedAt = bill.CreatedAt,
                PaidAt = bill.PaidAt
            });
        }

        /// <summary>
        /// Создать счёт для заказа
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BillDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BillDto>> CreateBill(CreateBillDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

            if (order == null)
                return BadRequest("Order not found");

            if (await _context.Bills.AnyAsync(b => b.OrderId == dto.OrderId))
                return BadRequest("Bill already exists for this order");

            var total = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);

            var bill = new Bill
            {
                OrderId = dto.OrderId,
                Total = total
            };

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBill), new { id = bill.Id }, new BillDto
            {
                Id = bill.Id,
                OrderId = bill.OrderId,
                Total = bill.Total,
                IsPaid = bill.IsPaid,
                CreatedAt = bill.CreatedAt,
                PaidAt = bill.PaidAt
            });
        }

        /// <summary>
        /// Оплатить счёт
        /// </summary>
        [HttpPatch("{id}/pay")]
        [ProducesResponseType(typeof(BillDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BillDto>> PayBill(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
                return NotFound();

            bill.IsPaid = true;
            bill.PaidAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new BillDto
            {
                Id = bill.Id,
                OrderId = bill.OrderId,
                Total = bill.Total,
                IsPaid = bill.IsPaid,
                CreatedAt = bill.CreatedAt,
                PaidAt = bill.PaidAt
            });
        }
    }
}
