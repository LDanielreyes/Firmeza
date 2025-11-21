using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Firmeza.Pages.Admin.Clients
{
    [Authorize(Roles = "Administrador")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Data.Entities.Client Client { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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

        public async Task<IActionResult> OnPostAsync()
        {
            // Do not validate Email/UserName inherited from IdentityUser here,
            // as changes to these require UserManager methods.
            // We only focus on Client-specific properties here.

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // We attach the object and mark it as modified to update the database
            _context.Attach(Client).State = EntityState.Modified;

            // Mark Email/UserName/etc. as not modified if they were not intended to be edited here
            // This prevents issues if these fields are not included in the form/BindProperty
            _context.Entry(Client).Property(c => c.Email).IsModified = false;
            _context.Entry(Client).Property(c => c.UserName).IsModified = false;
            _context.Entry(Client).Property(c => c.PasswordHash).IsModified = false;
            _context.Entry(Client).Property(c => c.NormalizedEmail).IsModified = false;
            _context.Entry(Client).Property(c => c.NormalizedUserName).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(Client.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ClientExists(int id)
        {
            return _context.Users.OfType<Data.Entities.Client>().Any(e => e.Id == id);
        }
    }
}