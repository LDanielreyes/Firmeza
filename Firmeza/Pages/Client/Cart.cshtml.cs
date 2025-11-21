using Firmeza.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firmeza.Interfaces;

namespace Firmeza.Pages.Client
{
    public class CartModel : PageModel
    {
        private readonly IShoppingCartService _cartService;

        public CartModel(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalGross { get; set; }
        public decimal TotalNet { get; set; }
        public decimal Iva { get; set; }

        public async Task OnGetAsync()
        {
            CartItems = await _cartService.GetCartItemsAsync();
            TotalNet = CartItems.Sum(i => i.SubTotal);
            const decimal IVARate = 0.19m;
            TotalGross = TotalNet * (1 + IVARate);
            Iva = TotalGross - TotalNet;
        }

        public async Task<IActionResult> OnPostRemove(int productId)
        {
            _cartService.RemoveItem(productId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdate(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                _cartService.RemoveItem(productId);
            }
            else
            {
                _cartService.RemoveItem(productId);
                _cartService.AddItem(productId, quantity);
            }

            return RedirectToPage();
        }
        
        public IActionResult OnPostClear()
        {
            _cartService.ClearCart();
            return RedirectToPage();
        }
    }
}