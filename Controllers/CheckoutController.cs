using EchoCrafter.Data;
using EchoCrafter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EchoCrafter.Models;

namespace EchoCrafter.Controllers
{
    [Route("checkout")]
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _db;
        private readonly CartService _cart;
        public CheckoutController(AppDbContext db, CartService cart)
        {
            _db = db;
            _cart = cart;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.Total = _cart.Total();
            return View();
        }

        [HttpPost("")]
        public async Task<IActionResult> Place(string customerName, string email, string address)
        {
            var items = _cart.GetCart();
            if (!items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var order = new Order
            {
                CustomerName = customerName,
                Email = email,
                Address = address,
                Total = items.Sum(i => i.UnitPrice * i.Quantity),
            };

            foreach (var i in items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                });
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            _cart.Clear();

            return RedirectToAction("Success", new { id = order.Id });
        }

        [HttpGet("success/{id:int}")]
        public async Task<IActionResult> Success(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }
    }
}
