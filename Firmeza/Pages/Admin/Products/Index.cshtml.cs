using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Pages.Admin.Products
{
    [Authorize(Roles = "Administrador")]
    public class IndexModel(ApplicationDbContext context) : PageModel
    {
        public IList<Product> Product { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Product = await context.Products.ToListAsync();
        }
    }
}