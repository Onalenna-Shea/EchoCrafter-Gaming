using EchoCrafter.Data;
using EchoCrafter.Services;
using Microsoft.AspNetCore.Mvc;

namespace EchoCrafter.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        public CartController(CartService cartService) => _cartService = cartService;

        [HttpGet("")]
        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            ViewBag.Total = _cartService.Total();
            return View(cart);
        }

        [HttpPost("add/{productId:int}")]
        public async Task<IActionResult> Add(int productId, int qty = 1)
        {
            await _cartService.AddAsync(productId, qty);
            return RedirectToAction("Index");
        }

        [HttpPost("remove/{productId:int}")]
        public IActionResult Remove(int productId)
        {
            _cartService.Remove(productId);
            return RedirectToAction("Index");
        }

        [HttpPost("update/{productId:int}")]
        public IActionResult Update(int productId, int qty)
        {
            _cartService.UpdateQuantity(productId, qty);
            return RedirectToAction("Index");
        }
    }
}
