using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CakeStore.Data;
using CakeStore.Models;
using System.Text.Json;
using System.Globalization;

namespace CakeStore.Pages
{
    public class OrderModel(CakeStoreContext context) : PageModel
    {
        private readonly CakeStoreContext _context = context;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty, Range(1, int.MaxValue, ErrorMessage = "You must order at elast one item")]
        public int Quantity { get; set; } = 1;

        [BindProperty]
        public decimal UnitPrice { get; set; }

        [TempData]
        public required string Confirmation { get; set; }

        public Product? Product { get; set; }

        public async Task OnGetAsync() => Product = await _context.Products.FindAsync(Id);

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                Basket? basket = null;
                string cookieBasket = Request.Cookies[nameof(Basket)] ?? string.Empty;
                if (!string.IsNullOrEmpty(cookieBasket))
                    basket = JsonSerializer.Deserialize<Basket>(cookieBasket);

                if (basket is null)
                    basket = new();

                basket.Items.Add(new OrderItem
                {
                    ProductId = Id,
                    UnitPrice = UnitPrice,
                    Quantity = Quantity
                });
                var json = JsonSerializer.Serialize(basket);
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30)
                };
                Response.Cookies.Append(nameof(Basket), json, cookieOptions);
                return RedirectToPage("/Index");
            }
            Product = await _context.Products.FindAsync(Id);
            return Page();
        }
    }
}
