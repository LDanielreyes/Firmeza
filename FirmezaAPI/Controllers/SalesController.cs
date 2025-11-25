using AutoMapper;
using Firmeza.Data;
using Firmeza.Data.Entities;
using FirmezaAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirmezaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SalesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptDto>>> GetSales()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role);

            if (userIdClaim == null) return Unauthorized();

            var query = _context.Receipts
                .Include(r => r.Client)
                .Include(r => r.SaleLines)
                .ThenInclude(s => s.Product)
                .OrderByDescending(r => r.ReceiptDate)
                .AsQueryable();

            if (userRoleClaim?.Value != "Admin")
            {
                if (int.TryParse(userIdClaim.Value, out int userId))
                {
                    query = query.Where(r => r.ClientId == userId);
                }
            }

            var receipts = await query.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ReceiptDto>>(receipts));
        }

        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptDto>> GetSale(int id)
        {
            var receipt = await _context.Receipts
                .Include(r => r.Client)
                .Include(r => r.SaleLines)
                .ThenInclude(s => s.Product)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt == null)
            {
                return NotFound();
            }

            // Check if user is admin or the owner of the receipt
            // For now, allowing all authenticated users to view (or restrict based on requirements)
            // To restrict: 
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // if (!User.IsInRole("Admin") && receipt.Client.Id.ToString() != userId) return Forbid();

            return Ok(_mapper.Map<ReceiptDto>(receipt));
        }

        // POST: api/Sales
        [HttpPost]
        public async Task<ActionResult<ReceiptDto>> PostSale(CreateSaleDto createSaleDto)
        {
            var client = await _context.People.OfType<Client>().FirstOrDefaultAsync(c => c.Id == createSaleDto.ClientId);
            if (client == null)
            {
                return BadRequest("Client not found");
            }

            var receipt = new Receipt
            {
                ClientId = createSaleDto.ClientId,
                ReceiptDate = DateTime.UtcNow,
                SaleLines = new List<Sale>()
            };

            decimal grossTotal = 0;

            foreach (var item in createSaleDto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Product with ID {item.ProductId} not found");
                }

                if (product.Stock < item.Quantity)
                {
                    return BadRequest($"Insufficient stock for product {product.Name}");
                }

                // Deduct stock
                product.Stock -= item.Quantity;

                var saleLine = new Sale
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PricePerUnit = product.Price,
                    NetTotal = product.Price * item.Quantity
                };

                receipt.SaleLines.Add(saleLine);
                grossTotal += saleLine.NetTotal;
            }

            receipt.GrossTotal = grossTotal;
            receipt.IvaTotal = grossTotal * 0.19m; // Assuming 19% IVA, adjust as needed

            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();

            // Reload receipt with includes for DTO mapping
            var createdReceipt = await _context.Receipts
                .Include(r => r.Client)
                .Include(r => r.SaleLines)
                .ThenInclude(s => s.Product)
                .FirstOrDefaultAsync(r => r.Id == receipt.Id);

            return CreatedAtAction("GetSale", new { id = receipt.Id }, _mapper.Map<ReceiptDto>(createdReceipt));
        }
    }
}
