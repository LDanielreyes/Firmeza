using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace Firmeza.Pages.Admin.Sales
{
    [Authorize(Roles = "Administrador")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- View Model for Line Items ---
        public class SaleLineInput
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        // --- Bind Properties for Form Submission ---
        
        [BindProperty]
        public int ClientId { get; set; }

        [BindProperty]
        public List<SaleLineInput> SubmittedItems { get; set; } = new List<SaleLineInput>();
        
        // --- Data for View ---

        public SelectList ClientOptions { get; set; } = default!;
        public List<Product> AvailableProducts { get; set; } = new List<Product>();

        // --- Handlers ---

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            ClientOptions = new SelectList(
                await _context.Users.OfType<Data.Entities.Client>().ToListAsync(), 
                nameof(Client.FullName), 
                nameof(Client.FullName));
            
            // Load products with sufficient stock
            AvailableProducts = await _context.Products
                .Where(p => p.Stock > 0)
                .ToListAsync();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDataAsync();

            if (ClientId == 0)
            {
                ModelState.AddModelError(string.Empty, "Client selection is mandatory.");
            }

            if (!SubmittedItems.Any(i => i.Quantity > 0))
            {
                ModelState.AddModelError(string.Empty, "The receipt must include at least one item.");
            }
            
            if (!ModelState.IsValid) return Page();

            // --- Calculation & Validation ---
            decimal grossTotal = 0;
            decimal ivaTotal = 0;
            var lineItems = new List<Sale>();
            
            foreach (var item in SubmittedItems.Where(i => i.Quantity > 0))
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null || product.Stock < item.Quantity)
                {
                    ModelState.AddModelError(string.Empty, $"Stock error for Product ID {item.ProductId}. Please re-check quantities.");
                    return Page();
                }
                
                // Assuming IVA is 19% (example rate)
                const decimal ivaRate = 0.19m; 
                decimal netTotal = item.Quantity * product.Price;
                decimal itemIva = netTotal * ivaRate;
                decimal itemGross = netTotal + itemIva;

                grossTotal += itemGross;
                ivaTotal += itemIva;

                lineItems.Add(new Sale
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PricePerUnit = product.Price,
                    NetTotal = netTotal 
                });
                
                // Decrease product stock
                product.Stock -= item.Quantity;
                _context.Entry(product).State = EntityState.Modified;
            }

            // --- Create Receipt ---
            var newReceipt = new Receipt
            {
                ClientId = ClientId,
                ReceiptDate = DateTime.UtcNow,
                GrossTotal = grossTotal,
                IvaTotal = ivaTotal,
                SaleLines = lineItems
            };

            _context.Receipts.Add(newReceipt);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = newReceipt.Id });
        }
    }
}   