using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Firmeza.Models;

namespace Firmeza.Pages.Admin.Clients
{
    [Authorize(Roles = "Administrador")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Person> _userManager;

        public DeleteModel(ApplicationDbContext context, UserManager<Person> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Data.Entities.Client Client { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the user, explicitly casting to Client
            var client = await _context.Users
                .OfType<Data.Entities.Client>()
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (client == null)
            {
                return NotFound();
            }
            
            Client = client;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the user using UserManager (necessary for identity operations)
            var clientToDelete = await _userManager.FindByIdAsync(id.ToString());
            
            if (clientToDelete != null)
            {
                // Delete the user via UserManager
                var result = await _userManager.DeleteAsync(clientToDelete);

                if (!result.Succeeded)
                {
                    // Handle deletion errors if necessary
                    return Page();
                }
            }

            return RedirectToPage("./Index");
        }
    }
}