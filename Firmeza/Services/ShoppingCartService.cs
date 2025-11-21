using Firmeza.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Firmeza.Interfaces;


namespace Firmeza.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private const string CartSessionKey = "CartItems";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public ShoppingCartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        private List<CartItem> GetCartFromSession()
        {
            var cartJson = Session.GetString(CartSessionKey);
            return cartJson == null
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        public void AddItem(int productId, int quantity = 1)
        {
            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem { ProductId = productId, Quantity = quantity });
            }

            SaveCartToSession(cart);
        }

        public void RemoveItem(int productId)
        {
            var cart = GetCartFromSession();
            cart.RemoveAll(i => i.ProductId == productId);
            SaveCartToSession(cart);
        }

        public void ClearCart()
        {
            Session.Remove(CartSessionKey);
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            var cart = GetCartFromSession();
            
            var productIds = cart.Select(i => i.ProductId).ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

            foreach (var item in cart)
            {
                item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
            }

            return cart.Where(i => i.Product != null).ToList();
        }

        public int GetTotalItemsCount()
        {
             return GetCartFromSession().Sum(i => i.Quantity);
        }
    }
}