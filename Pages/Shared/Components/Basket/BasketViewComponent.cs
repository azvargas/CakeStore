using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace CakeStore.Pages.Shared.Components.Basket
{
    public class BasketViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            Models.Basket? basket = new();
            string cookieBasket = Request.Cookies[nameof(Basket)] ?? string.Empty;
            if (!string.IsNullOrEmpty(cookieBasket))
            {
                basket = JsonSerializer.Deserialize<Models.Basket?>(cookieBasket!);
            }
            return View(basket);
        }

        /*
    private readonly BakeryContext context;
    public BasketViewComponent(BakeryContext context)
    {
        this.context = context;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var data = await context.Products.ToListAsync();
        return View(data);
    }         */
    }
}
