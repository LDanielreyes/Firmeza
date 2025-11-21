using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Firmeza.Interfaces;
using Firmeza.Models;

namespace Firmeza.Pages.Client
{
    [Authorize(Roles = "Cliente")]
    public class CheckoutModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IShoppingCartService _cartService;
        private readonly UserManager<Person> _userManager;

        public CheckoutModel(ApplicationDbContext context, IShoppingCartService cartService, UserManager<Person> userManager)
        {
            _context = context;
            _cartService = cartService;
            _userManager = userManager;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalGross { get; set; }
        public Data.Entities.Client CurrentClient { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is not Data.Entities.Client client) return Unauthorized();
            CurrentClient = client;

            CartItems = await _cartService.GetCartItemsAsync();
            if (!CartItems.Any()) return RedirectToPage("/Client/Catalog");

            var totalNet = CartItems.Sum(i => i.SubTotal);
            TotalGross = totalNet * 1.19m;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is not Data.Entities.Client client) return Unauthorized();

            var cartItems = await _cartService.GetCartItemsAsync();
            if (!cartItems.Any()) return RedirectToPage("/Client/Catalog");

            // Validate Stock
            foreach (var item in cartItems)
            {
                if (item.Product!.Stock < item.Quantity)
                {
                    ModelState.AddModelError(string.Empty, $"Insufficient stock for {item.Product.Name}");
                    return await OnGetAsync(); // Reload page with error
                }
            }

            decimal netTotal = 0;
            var saleLines = new List<Sale>();

            foreach (var item in cartItems)
            {
                // Fetch latest product data to ensure price/stock integrity
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null) continue;

                decimal lineNet = item.Quantity * product.Price;
                netTotal += lineNet;

                saleLines.Add(new Sale
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PricePerUnit = product.Price,
                    NetTotal = lineNet
                });

                product.Stock -= item.Quantity;
                _context.Products.Update(product);
            }

            decimal ivaTotal = netTotal * 0.19m;
            decimal grossTotal = netTotal + ivaTotal;

            var receipt = new Receipt
            {
                ClientId = client.Id,
                ReceiptDate = DateTime.UtcNow,
                GrossTotal = grossTotal,
                IvaTotal = ivaTotal,
                SaleLines = saleLines
            };

            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();

            _cartService.ClearCart();

            return RedirectToPage("/Admin/Sales/Details", new { id = receipt.Id });
        }
    }
}