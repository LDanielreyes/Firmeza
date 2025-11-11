using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace Firmeza.Pages.Admin.Sales
{
    [Authorize(Roles = "Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Receipt> ReceiptList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Eagerly load the Client (Person) who made the purchase
            ReceiptList = await _context.Receipts
                .Include(r => r.Client)
                .OrderByDescending(r => r.ReceiptDate)
                .ToListAsync();
        }
    }
}