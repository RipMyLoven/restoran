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
    public class InvoicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("orders/{orderId}/invoice")]
        [Authorize(Roles = "Admin,Manager,Waiter")]
        public async Task<ActionResult<InvoiceDto>> CreateInvoice(int orderId, CreateInvoiceDto createInvoiceDto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            // Check if invoice already exists
            var existingInvoice = await _context.Set<Invoice>().FirstOrDefaultAsync(i => i.OrderId == orderId);
            if (existingInvoice != null)
            {
                return BadRequest("Invoice already exists for this order");
            }

            // Calculate totals
            var subtotal = order.OrderItems.Sum(oi => oi.PriceAtOrder * oi.Quantity);
            var tax = subtotal * 0.10m; // 10% tax
            var total = subtotal + tax;

            var invoice = new Invoice
            {
                OrderId = orderId,
                InvoiceNumber = GenerateInvoiceNumber(),
                Subtotal = subtotal,
                Tax = tax,
                Total = total,
                DueDate = createInvoiceDto.DueDate ?? DateTime.UtcNow.AddDays(30),
                Notes = createInvoiceDto.Notes
            };

            _context.Set<Invoice>().Add(invoice);
            await _context.SaveChangesAsync();

            var invoiceDto = new InvoiceDto
            {
                Id = invoice.Id,
                OrderId = invoice.OrderId,
                InvoiceNumber = invoice.InvoiceNumber,
                Subtotal = invoice.Subtotal,
                Tax = invoice.Tax,
                Total = invoice.Total,
                CreatedAt = invoice.CreatedAt,
                DueDate = invoice.DueDate,
                IsPaid = invoice.IsPaid,
                PaidAt = invoice.PaidAt,
                PaymentMethod = invoice.PaymentMethod,
                Notes = invoice.Notes
            };

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoiceDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoices([FromQuery] bool? isPaid = null)
        {
            var query = _context.Set<Invoice>().AsQueryable();

            if (isPaid.HasValue)
            {
                query = query.Where(i => i.IsPaid == isPaid.Value);
            }

            var invoices = await query
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    OrderId = i.OrderId,
                    InvoiceNumber = i.InvoiceNumber,
                    Subtotal = i.Subtotal,
                    Tax = i.Tax,
                    Total = i.Total,
                    CreatedAt = i.CreatedAt,
                    DueDate = i.DueDate,
                    IsPaid = i.IsPaid,
                    PaidAt = i.PaidAt,
                    PaymentMethod = i.PaymentMethod,
                    Notes = i.Notes
                }).ToListAsync();

            return Ok(invoices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
        {
            var invoice = await _context.Set<Invoice>()
                .Include(i => i.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                .Where(i => i.Id == id)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    OrderId = i.OrderId,
                    InvoiceNumber = i.InvoiceNumber,
                    Subtotal = i.Subtotal,
                    Tax = i.Tax,
                    Total = i.Total,
                    CreatedAt = i.CreatedAt,
                    DueDate = i.DueDate,
                    IsPaid = i.IsPaid,
                    PaidAt = i.PaidAt,
                    PaymentMethod = i.PaymentMethod,
                    Notes = i.Notes,
                    Order = new OrderDto
                    {
                        Id = i.Order.Id,
                        TableId = i.Order.TableId,
                        RestaurantId = i.Order.RestaurantId,
                        Status = i.Order.Status,
                        SpecialRequirements = i.Order.SpecialRequirements,
                        CustomerName = i.Order.CustomerName,
                        CreatedAt = i.Order.CreatedAt,
                        OrderItems = i.Order.OrderItems.Select(oi => new OrderItemDto
                        {
                            Id = oi.Id,
                            MenuItemId = oi.MenuItemId,
                            MenuItemName = oi.MenuItem.Name,
                            Quantity = oi.Quantity,
                            Price = oi.PriceAtOrder,
                            SpecialRequirements = oi.SpecialInstructions
                        }).ToList()
                    }
                }).FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }
}
