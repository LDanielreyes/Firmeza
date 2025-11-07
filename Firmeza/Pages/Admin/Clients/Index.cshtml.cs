using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Pages.Admin.Clients
{
    [Authorize(Roles = "Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // List will only contain Client objects due to the .OfType<Client>() filter
        public IList<Client> ClientList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Fetch all users that are explicitly Clients
            ClientList = await _context.Users
                .OfType<Client>()
                .ToListAsync();
        }
    }
}