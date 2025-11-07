using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Pages.Admin.Dashboard
{
    [Authorize(Roles = "Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Key Metrics
        public int TotalProducts { get; set; }
        public int TotalClients { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalSalesCount { get; set; }

        public async Task OnGetAsync()
        {
            // 1. Total Products
            TotalProducts = await _context.Products.CountAsync();

            // 2. Total Clients (Users of type Client)
            TotalClients = await _context.Users.OfType<Client>().CountAsync();

            // 3. Total Sales and Revenue
            var salesData = await _context.Sales
                .Select(s => s.NetTotal)
                .ToListAsync();

            TotalRevenue = salesData.Sum();
            TotalSalesCount = salesData.Count;
        }
    }
}