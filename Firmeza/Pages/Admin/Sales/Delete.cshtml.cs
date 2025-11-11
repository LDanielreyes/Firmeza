using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Firmeza.Pages.Admin.Sales
{
    [Authorize(Roles = "Administrador")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Receipt Receipt { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Cargar la Receipt, incluyendo el Cliente para la visualización de la confirmación
            var receipt = await _context.Receipts
                .Include(r => r.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (receipt == null)
            {
                return NotFound();
            }

            Receipt = receipt;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipt = await _context.Receipts.FindAsync(id);

            if (receipt != null)
            {
                // Se elimina la cabecera (Receipt); las líneas de venta (Sale) se eliminan automáticamente
                _context.Receipts.Remove(receipt);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}