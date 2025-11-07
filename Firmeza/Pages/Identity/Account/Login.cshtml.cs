using System.ComponentModel.DataAnnotations;
using Firmeza.Data.Entities;
using Firmeza.Models; // Asume que Person está en esta ubicación
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Pages.Identity.Account
{
    public class LoginModel(
        SignInManager<Person> signInManager, 
        ILogger<LoginModel> logger)
        : PageModel
    {

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel(); 
        public string? ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }
        
        public class InputModel
        {
            [Required(ErrorMessage = "El email es obligatorio.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [DataType(DataType.Password)]
            public string? Password { get; set; }
            
            [Display(Name = "Recordarme")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            ReturnUrl = returnUrl;
        }
        
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid) return Page();

            if (Input.Email != null)
            {
                if (Input.Password != null)
                {
                    var result = await signInManager.PasswordSignInAsync(
                        Input.Email,
                        Input.Password, 
                        Input.RememberMe, 
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        logger.LogInformation("Usuario {Email} inició sesión.", Input.Email);
                        return RedirectToPage("/Admin/Dashboard/Index");
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido: Email o contraseña incorrectos.");
            return Page();
        }
    }
}