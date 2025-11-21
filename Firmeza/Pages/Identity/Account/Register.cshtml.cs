using Firmeza.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Firmeza.Models; 

namespace Firmeza.Pages.Identity.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<Person> _userManager;
        private readonly SignInManager<Person> _signInManager;
        
        private const string ClientRole = "Cliente";

        public RegisterModel(
            UserManager<Person> userManager,
            SignInManager<Person> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
            public string Password { get; set; } = string.Empty;
            
            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            
            [Display(Name = "Document ID")]
            public string Document { get; set; } = string.Empty;
            
            public string Address { get; set; } = string.Empty;

            [Display(Name = "Phone")]
            public string Phone { get; set; } = string.Empty;
            
            public byte Age { get; set; } = 18; 
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new Data.Entities.Client() 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email, 
                    FullName = Input.FullName,
                    EmailConfirmed = true,
                    RegisterDate = DateTime.UtcNow, 
                    Phone = Input.Phone, 
                    Document = Input.Document,
                    Age = Input.Age,
                    Address = Input.Address,
                };
                
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, ClientRole);
                    
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            return Page();
        }
    }
}