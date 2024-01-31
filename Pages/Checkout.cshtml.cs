using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using CakeStore.Data;
using CakeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;

namespace CakeStore.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly CakeStoreContext _context;

        public CheckoutModel(CakeStoreContext context)
        {
            _context = context;
        }

        public Basket? Basket { get; set; } = new();

        public List<Product> SelectedProducts { get; set; } = new();

        [BindProperty, Required, Display(Name = "Your Email Addres")]
        public required string OrderEmail { get; set; }

        [BindProperty, Required, Display(Name = "Shipping Address")]
        public required string ShippingAddress { get; set; }

        [TempData]
        public required string Confirmation { get; set; }

        public async Task OnGetAsync()
        {
            string cookieBasket = Request.Cookies[nameof(Basket)] ?? string.Empty;
            if (!string.IsNullOrEmpty(cookieBasket))
            {
                Basket = JsonSerializer.Deserialize<Basket>(cookieBasket);
                if (Basket is not null && Basket.NumberOfItems > 0)
                {
                    var selectedProducts = Basket.Items.Select(x => x.ProductId).ToArray();
                    SelectedProducts = await _context.Products.Where(p => selectedProducts.Contains(p.Id)).ToListAsync();
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string cookieBasket = Request.Cookies[nameof(Basket)] ?? string.Empty;
            if (ModelState.IsValid && !string.IsNullOrEmpty(cookieBasket))
            {
                var basket = JsonSerializer.Deserialize<Basket>(cookieBasket);
                if (basket is not null)
                {
                    var plural = basket.NumberOfItems == 1 ? string.Empty : "s";
                    Confirmation = $@"<p>Your order for {basket.NumberOfItems} item{plural} has been received and is being processed:</p>
<p>It will be send to {ShippingAddress}. We will notify you when it has been dispatched</p>";

                    var message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse("test@test.com"));
                    message.To.Add(MailboxAddress.Parse(OrderEmail));
                    message.Subject = "Your order confirmation";
                    message.Body = new TextPart("html")
                    {
                        Text = Confirmation
                    };

                    using var client = new SmtpClient();
                    await client.ConnectAsync("localhost");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    Response.Cookies.Append(nameof(Basket), string.Empty, new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
                    return RedirectToPage("/OrderSuccess");
                }
            }
            return Page();
        }
    }
}
