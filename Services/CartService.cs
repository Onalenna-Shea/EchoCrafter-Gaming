using System.Text.Json;
using EchoCrafter.Data;
using EchoCrafter.Models;
using Microsoft.EntityFrameworkCore;

namespace EchoCrafter.Services
{
    public class CartService
    {
        private const string CartKey = "CART_V1";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _db;

        public CartService(IHttpContextAccessor httpContextAccessor, AppDbContext db)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public List<CartItem> GetCart()
        {
            var json = Session.GetString(CartKey);
            return string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : (JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>());
        }

        private void SaveCart(List<CartItem> items)
        {
            Session.SetString(CartKey, JsonSerializer.Serialize(items));
        }

        public async Task AddAsync(int productId, int qty = 1)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return;

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existing == null)
            {
                cart.Add(new CartItem {
                    ProductId = product.Id,
                    Name = product.Name,
                    UnitPrice = product.Price,
                    Quantity = qty,
                    ImageUrl = product.ImageUrl
                });
            }
            else
            {
                existing.Quantity += qty;
            }
            SaveCart(cart);
        }

        public void Remove(int productId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.ProductId == productId);
            SaveCart(cart);
        }

        public void UpdateQuantity(int productId, int qty)
        {
            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existing != null)
            {
                existing.Quantity = Math.Max(1, qty);
                SaveCart(cart);
            }
        }

        public void Clear()
        {
            SaveCart(new List<CartItem>());
        }

        public decimal Total() => GetCart().Sum(i => i.UnitPrice * i.Quantity);
    }
}
