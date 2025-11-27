using AutoMapper;
using Firmeza.Data;
using Firmeza.Data.Entities;
using FirmezaAPI.DTOs;
using FirmezaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirmezaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ProductImportService _importService;
        private readonly ProductExportService _exportService;

        public ProductsController(ApplicationDbContext context, IMapper mapper,
            ProductImportService importService, ProductExportService exportService)
        {
            _context = context;
            _mapper = mapper;
            _importService = importService;
            _exportService = exportService;
        }

        // GET: api/Products
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
            [FromQuery] string? search,
            [FromQuery] string? type,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()) || (p.Description != null && p.Description.ToLower().Contains(search.ToLower())));
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(p => p.Type == type);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var products = await query.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductDto>(product));
        }

        // POST: api/Products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var createdDto = _mapper.Map<ProductDto>(product);

            return CreatedAtAction("GetProduct", new { id = product.Id }, createdDto);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProduct(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _mapper.Map(productDto, product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost("import")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ImportResultDto>> ImportProducts(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado ningún archivo.");

            using var stream = file.OpenReadStream();
            var result = await _importService.ImportFromExcelAsync(stream);
            return Ok(result);
        }

        [HttpGet("export/excel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportProductsExcel()
        {
            var content = await _exportService.ExportToExcelAsync();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "productos.xlsx");
        }

        [HttpGet("export/pdf")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportProductsPdf()
        {
            var content = await _exportService.ExportToPdfAsync();
            return File(content, "application/pdf", "productos.pdf");
        }
    }
}
