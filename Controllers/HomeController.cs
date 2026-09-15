using EchoCrafter.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EchoCrafter.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? category)
        {
            var query = _db.Products.Include(p => p.Category).AsQueryable();
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category!.Slug == category);
                ViewBag.ActiveCategory = category;
            }
            var categories = await _db.Categories.ToListAsync();
            ViewBag.Categories = categories;
            return View(await query.OrderBy(p => p.Name).ToListAsync());
        }
    }
}
