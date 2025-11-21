using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Firmeza.Models;

namespace Firmeza.Pages.Admin.Clients
{
    [Authorize(Roles = "Administrador")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Person> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<Person> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public ClientInputModel ClientInput { get; set; } = new ClientInputModel();

        public class ClientInputModel
        {
            // Propiedades de Person
            [Required]
            [StringLength(100)]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
            public string Password { get; set; } = string.Empty;

            // Propiedades de Client
            [Required]
            [MaxLength(20)]
            [Display(Name = "Phone")]
            public string Phone { get; set; } = string.Empty;

            [Required]
            [MaxLength(20)]
            [Display(Name = "Document ID")]
            public string Document { get; set; } = string.Empty;

            [MaxLength(200)]
            [Display(Name = "Address")]
            public string Address { get; set; } = string.Empty;

            [Required]
            [Range(18, 120)]
            [Display(Name = "Age")]
            public byte Age { get; set; }
        }

        public void OnGet()
        {
            // La fecha de registro se establece en la entidad Client por defecto
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var newClient = new Data.Entities.Client
            {
                UserName = ClientInput.Email,
                Email = ClientInput.Email,
                FullName = ClientInput.FullName,
                EmailConfirmed = true,
                
                // Propiedades específicas de Client
                Phone = ClientInput.Phone,
                Document = ClientInput.Document,
                Address = ClientInput.Address,
                Age = ClientInput.Age,
            };
            
            var result = await _userManager.CreateAsync(newClient, ClientInput.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newClient, "Cliente");
                
                return RedirectToPage("./Index");
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}