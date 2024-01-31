using CakeStore.Data;
using CakeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CakeStore.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly CakeStoreContext _context;

    public IndexModel(ILogger<IndexModel> logger, CakeStoreContext context)
    {
        _logger = logger;
        _context = context;
    }

    public List<Product> Products { get; set; } = new();

    public async Task OnGetAsync() => Products = await _context.Products.ToListAsync();
   
}
