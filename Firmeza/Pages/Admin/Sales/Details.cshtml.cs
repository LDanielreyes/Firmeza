using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Pages.Admin.Sales
{
    [Authorize(Roles = "Administrador")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Receipt Receipt { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            // Eagerly load Client, SaleLines (Line Items), and the Product within SaleLines
            var receipt = await _context.Receipts
                .Include(r => r.Client)
                .Include(r => r.SaleLines)!
                .ThenInclude(sl => sl.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (receipt == null) return NotFound();
            
            Receipt = receipt;
            return Page();
        }
    }
}