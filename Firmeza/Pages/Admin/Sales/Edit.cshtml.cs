using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firmeza.Pages.Admin.Sales
{
    [Authorize(Roles = "Administrador")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Receipt Receipt { get; set; } = default!;

        public SelectList ClientOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var receipt = await _context.Receipts
                .Include(r => r.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (receipt == null) return NotFound();
            
            Receipt = receipt;

            // Cargar clientes para el selector
            ClientOptions = new SelectList(
                await _context.Users.OfType<Client>().ToListAsync(), 
                nameof(Client.Id), 
                nameof(Client.FullName));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Omitir la validación de ModelState aquí si las líneas de venta no están en el formulario
            // y solo nos enfocamos en los campos del Receipt.
            
            if (!ModelState.IsValid)
            {
                ClientOptions = new SelectList(
                    await _context.Users.OfType<Client>().ToListAsync(), 
                    nameof(Client.Id), 
                    nameof(Client.FullName));
                return Page();
            }

            _context.Attach(Receipt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Receipts.Any(e => e.Id == Receipt.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = Receipt.Id });
        }
    }
}