using EchoCrafter.Data;
using EchoCrafter.Models;
using EchoCrafter.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=echoCrafter.db"));

builder.Services.AddDistributedMemoryCache();

string v = "EchoCrafter.Session";

builder.Services.AddSession(options =>
{
    options.Cookie.Name = v;
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CartService>();

var app = builder.Build();

// Ensure DB created + seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Categories.Any())
    {
        var hoodies = new Category { Name = "Hoodies", Slug = "hoodies" };
        var tshirt = new Category { Name = "T-Shirts", Slug = "t-shirts" };
        var jackets = new Category { Name = "Jackets", Slug = "jackets" };
        var combos = new Category { Name = "Combo", Slug = "combo" };
        var headwear = new Category { Name = "Head Wear", Slug = "headwear" };
        db.Categories.AddRange(hoodies, tshirt, jackets, combos, headwear);
        db.SaveChanges();

        db.Products.AddRange(
           new Product { Name = "Combo 1", Slug = "Combo-1", Price = 750.00m, CategoryId = combos.Id, ImageUrl = "/img/Combo1.jpeg", Description = "All white echoCrafter hoodie and t-shirt" },
            new Product { Name = "Warm Jacket", Slug = "Warm-Jacket", Price = 830.00m, CategoryId = jackets.Id, ImageUrl = "/img/puffy-jacket.jpeg", Description = "Cozy warmy echoCrafter jacket." },
            new Product { Name = "Warm Jersey", Slug = "Warm-Jersey", Price = 499.99m, CategoryId = jackets.Id, ImageUrl = "/img/jersey.jpeg", Description = "Warmy echoCrafter jersey." },
            new Product { Name = "Hoodies", Slug = "Hoodies", Price = 499.99m, CategoryId = hoodies.Id, ImageUrl = "/img/Hoodie2.jpeg", Description = "Warm hoodies." },
            new Product { Name = "T-Shirt", Slug = "T_Shirt-1", Price = 300.00m, CategoryId = tshirt.Id, ImageUrl = "/img/T-Shirt.jpeg", Description = "EchoCrafter customised T-shirt" },
            new Product { Name = "Golf T-Shirt", Slug = "Golf-T_Shirt", Price = 349.99m, CategoryId = tshirt.Id, ImageUrl = "/img/Golf T-shirt.jpeg", Description = "EchoCrafter customised golf shirt" },
            new Product { Name = "Golf T-Shirt", Slug = "Golf-Shirt", Price = 349.99m, CategoryId = tshirt.Id, ImageUrl = "/img/golf-shirt.jpeg", Description = "EchoCrafter customised golf shirt" },
            new Product { Name = "Combo 2", Slug = "Combo-2", Price = 1799.00m, CategoryId = combos.Id, ImageUrl = "/img/Combo2.jpeg", Description = "All black echoCrafter hoodie and t-shirt" },
            new Product { Name = "White Hoodie", Slug = "White-Hoodie", Price = 499.99m, CategoryId = hoodies.Id, ImageUrl = "/img/White Hoodie.jpeg", Description = "White customised hoodie" },
            new Product { Name = "Head Wear", Slug = "headwear1", Price = 149.99m, CategoryId = headwear.Id, ImageUrl = "/img/Cap.jpeg", Description = "EchoCrafter head-wear" },
            new Product { Name = "Head Wear", Slug = "headwear", Price = 149.99m, CategoryId = headwear.Id, ImageUrl = "/img/head-wear.jpeg", Description = "EchoCrafter head-wear" }
        );
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

