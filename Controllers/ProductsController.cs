using EchoCrafter.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EchoCrafter.Controllers
{
    [Route("products")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db) => _db = db;

        [HttpGet("{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Slug == slug);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}
