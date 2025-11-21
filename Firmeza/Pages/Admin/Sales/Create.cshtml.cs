using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Firmeza.Pages.Admin.Sales
{
    [Authorize(Roles = "Administrador")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private const decimal IVARate = 0.19m;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // --- Simplified Input Model for a Single Line Item ---
        public class SingleSaleInput
        {
            [Required]
            [Display(Name = "Product")]
            public int ProductId { get; set; }
            
            [Required]
            [Range(1, 1000)]
            public int Quantity { get; set; }
        }

        [BindProperty]
        [Display(Name = "Client Full Name")]
        [Required(ErrorMessage = "Client selection is mandatory.")]
        public string ClientFullName { get; set; } = string.Empty; 

        [BindProperty]
        public SingleSaleInput Item { get; set; } = new SingleSaleInput();
        
        public SelectList ClientOptions { get; set; } = default!;
        public SelectList ProductOptions { get; set; } = default!;

        public async Task OnGetAsync() => await LoadDataAsync();

        private async Task LoadDataAsync()
        {
            ClientOptions = new SelectList(
                await _context.Users.OfType<Data.Entities.Client>().ToListAsync(), 
                nameof(Data.Entities.Client.FullName), 
                nameof(Data.Entities.Client.FullName));
            
            ProductOptions = new SelectList(
                await _context.Products.Where(p => p.Stock > 0).ToListAsync(),
                nameof(Product.Id),
                nameof(Product.Name)
            );
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDataAsync();

            if (!ModelState.IsValid) return Page();

            // 1. BUSCAR CLIENTE por FullName
            var client = await _context.Users
                .OfType<Data.Entities.Client>()
                .FirstOrDefaultAsync(c => c.FullName == ClientFullName);
            
            if (client == null)
            {
                ModelState.AddModelError(nameof(ClientFullName), "Client not found with the provided name.");
                return Page();
            }

            // 2. Validar y Obtener Producto
            var product = await _context.Products.FindAsync(Item.ProductId);

            if (product == null)
            {
                ModelState.AddModelError(nameof(Item.ProductId), "Invalid product selected.");
                return Page();
            }

            if (product.Stock < Item.Quantity)
            {
                ModelState.AddModelError(nameof(Item.Quantity), $"Insufficient stock. Available: {product.Stock}");
                return Page();
            }
            
            // 3. Cálculo de Totales
            decimal netTotal = Item.Quantity * product.Price;
            decimal ivaTotal = netTotal * IVARate;
            decimal grossTotal = netTotal + ivaTotal;
            
            var saleLine = new Sale() 
            {
                ProductId = Item.ProductId, // Sourced from the input model
                Quantity = Item.Quantity, // Sourced from the input model
                PricePerUnit = product.Price, // Sourced from the DB product price
                NetTotal = netTotal 
            };
            
            // 5. Crear Recibo
            var newReceipt = new Receipt
            {
                ClientId = client.Id, 
                ReceiptDate = DateTime.UtcNow,
                GrossTotal = grossTotal,
                IvaTotal = ivaTotal,
                SaleLines = new List<Sale> { saleLine }
            };

            // 6. Actualizar Stock
            product.Stock -= Item.Quantity;
            _context.Entry(product).State = EntityState.Modified;

            // 7. Guardar en Base de Datos
            _context.Receipts.Add(newReceipt);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = newReceipt.Id });
        }
    }
}