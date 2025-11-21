using Firmeza.Data;
using Firmeza.Data.Entities;
using Firmeza.Services; // Ensure this is present for ReceiptDocument and QuestPDF
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent; // Necessary for the .GeneratePdf() extension method
using System.IO;
using System.Threading.Tasks;

namespace Firmeza.Pages.Admin.Sales
{
    // [Authorize(Roles = "Administrador")]
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

            var receipt = await _context.Receipts
                .Include(r => r.Client)
                .Include(r => r.SaleLines)!
                .ThenInclude(sl => sl.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (receipt == null) return NotFound();
            
            Receipt = receipt;
            return Page();
        }

        // CORRECCIÓN: Handler que utiliza la clase ReceiptDocument y QuestPDF
        public async Task<IActionResult> OnGetGeneratePdf(int? id)
        {
            if (id == null) return NotFound();

            // Cargar datos necesarios (igual que OnGetAsync)
            var receipt = await _context.Receipts
                .Include(r => r.Client)
                .Include(r => r.SaleLines)!
                .ThenInclude(sl => sl.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (receipt == null) return NotFound();

            // 1. Crear el documento QuestPDF, inyectando el objeto Receipt
            var document = new ReceiptDocument(receipt);

            // 2. Renderizar el documento a un array de bytes
            var pdfBytes = document.GeneratePdf();

            // 3. Devolver el archivo como FileStreamResult (ya que QuestPDF devuelve bytes)
            return File(
                pdfBytes, 
                "application/pdf", 
                $"Admin_Receipt_{receipt.Id}_{receipt.ReceiptDate.Year}.pdf"
            );
        }
        
        // El método CreateCell ya no es necesario aquí, ya que la lógica está en ReceiptDocument.cs
    }
}