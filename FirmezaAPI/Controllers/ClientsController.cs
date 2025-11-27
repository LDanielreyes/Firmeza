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
    [Authorize(Roles = "Admin")] // Default to Admin only for client management
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ClientImportService _importService;
        private readonly ClientExportService _exportService;

        public ClientsController(ApplicationDbContext context, IMapper mapper,
            ClientImportService importService, ClientExportService exportService)
        {
            _context = context;
            _mapper = mapper;
            _importService = importService;
            _exportService = exportService;
        }

        // GET: api/Clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var clients = await _context.People.OfType<Client>().ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ClientDto>>(clients));
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClient(int id)
        {
            var client = await _context.People.OfType<Client>().FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ClientDto>(client));
        }

        // POST: api/Clients
        [HttpPost]
        public async Task<ActionResult<ClientDto>> PostClient(ClientDto clientDto)
        {
            // Note: Usually clients are created via Auth/Register. 
            // This endpoint might be for Admin to manually add a client without password (if allowed) or with default password.
            // For simplicity, we'll assume this creates a Client entity, but we need to handle IdentityUser properties.
            // This is complex because Client inherits Person (IdentityUser).
            // It's better to use UserManager to create clients.
            
            return BadRequest("Please use Auth/Register to create new clients.");
        }

        // PUT: api/Clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, ClientDto clientDto)
        {
            if (id != clientDto.Id)
            {
                return BadRequest();
            }

            var client = await _context.People.OfType<Client>().FirstOrDefaultAsync(c => c.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            _mapper.Map(clientDto, client);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
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

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.People.OfType<Client>().FirstOrDefaultAsync(c => c.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            _context.People.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.People.OfType<Client>().Any(e => e.Id == id);
        }

        [HttpPost("import")]
        public async Task<ActionResult<ImportResultDto>> ImportClients(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado ningún archivo.");

            using var stream = file.OpenReadStream();
            var result = await _importService.ImportFromExcelAsync(stream);
            return Ok(result);
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportClientsExcel()
        {
            var content = await _exportService.ExportToExcelAsync();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "clientes.xlsx");
        }

        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportClientsPdf()
        {
            var content = await _exportService.ExportToPdfAsync();
            return File(content, "application/pdf", "clientes.pdf");
        }
    }
}
