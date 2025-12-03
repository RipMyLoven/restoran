using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restoran.Data;
using Restoran.Models;

namespace Restoran.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Bills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> GetBills()
        {
            return await _context.Bills
                .Include(b => b.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                .ToListAsync();
        }

        // GET: api/Bills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetBill(int id)
        {
            var bill = await _context.Bills
                .Include(b => b.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null)
            {
                return NotFound();
            }

            return bill;
        }

        // GET: api/Bills/order/5
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<Bill>> GetBillByOrder(int orderId)
        {
            var bill = await _context.Bills
                .Include(b => b.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(b => b.OrderId == orderId);

            if (bill == null)
            {
                return NotFound();
            }

            return bill;
        }

        // POST: api/Bills/generate/5
        [HttpPost("generate/{orderId}")]
        public async Task<ActionResult<Bill>> GenerateBill(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            var existingBill = await _context.Bills.FirstOrDefaultAsync(b => b.OrderId == orderId);
            if (existingBill != null)
            {
                return BadRequest("Bill already exists for this order");
            }

            decimal subtotal = order.OrderItems.Sum(oi => oi.PriceAtOrder * oi.Quantity);
            decimal tax = subtotal * 0.20m;
            decimal total = subtotal + tax;

            var bill = new Bill
            {
                OrderId = orderId,
                Subtotal = subtotal,
                Tax = tax,
                Total = total,
                CreatedAt = DateTime.Now,
                IsPaid = false
            };

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBill", new { id = bill.Id }, bill);
        }

        // POST: api/Bills/5/pay
        [HttpPost("{id}/pay")]
        public async Task<IActionResult> PayBill(int id, [FromBody] PaymentRequest request)
        {
            var bill = await _context.Bills
                .Include(b => b.Order)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null)
            {
                return NotFound();
            }

            if (bill.IsPaid)
            {
                return BadRequest("Bill is already paid");
            }

            bill.IsPaid = true;
            bill.PaidAt = DateTime.Now;
            bill.PaymentMethod = request.PaymentMethod;

            bill.Order.Status = OrderStatus.Completed;
            bill.Order.CompletedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment successful", billId = id });
        }

        // PUT: api/Bills/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBill(int id, Bill bill)
        {
            if (id != bill.Id)
            {
                return BadRequest();
            }

            _context.Entry(bill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Bills
        [HttpPost]
        public async Task<ActionResult<Bill>> PostBill(Bill bill)
        {
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBill", new { id = bill.Id }, bill);
        }

        // DELETE: api/Bills/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BillExists(int id)
        {
            return _context.Bills.Any(e => e.Id == id);
        }
    }

    public class PaymentRequest
    {
        public string PaymentMethod { get; set; } = string.Empty;
    }
}