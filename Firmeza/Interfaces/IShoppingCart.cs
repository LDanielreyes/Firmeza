using Firmeza.Data.Entities;

namespace Firmeza.Interfaces
{
    public interface IShoppingCartService
    {
        void AddItem(int productId, int quantity = 1);
        void RemoveItem(int productId);
        void ClearCart();
        Task<List<CartItem>> GetCartItemsAsync();
        int GetTotalItemsCount();
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }
        public decimal SubTotal => Product != null ? Product.Price * Quantity : 0;
    }
}