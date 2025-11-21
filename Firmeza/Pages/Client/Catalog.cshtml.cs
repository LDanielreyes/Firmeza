using Firmeza.Data;
using Firmeza.Data.Entities;
using Firmeza.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firmeza.Interfaces;

namespace Firmeza.Pages.Client
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IShoppingCartService _cartService;

        public IndexModel(ApplicationDbContext context, IShoppingCartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public IList<Product> Products { get; set; } = default!;
        public int CartItemCount { get; set; }

        public async Task OnGetAsync()
        {
            Products = await _context.Products
                .Where(p => p.Stock > 0)
                .ToListAsync();
                
            CartItemCount = _cartService.GetTotalItemsCount();
        }

        public IActionResult OnPostAddToCart(int productId, int quantity = 1)
        {
            if (quantity < 1) return BadRequest();

            _cartService.AddItem(productId, quantity);

            return RedirectToPage(); 
        }
    }
}