using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CakeStore.Data;
using CakeStore.Models;

namespace CakeStore.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly CakeStore.Data.CakeStoreContext _context;

        public IndexModel(CakeStore.Data.CakeStoreContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Products != null)
            {
                Product = await _context.Products.ToListAsync();
            }
        }
    }
}
