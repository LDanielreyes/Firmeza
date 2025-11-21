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
    [Authorize(Roles = "Admin")] // Default to Admin only for client management
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ClientsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
    }
}
