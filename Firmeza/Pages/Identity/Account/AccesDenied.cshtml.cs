using Firmeza.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Pages.Identity.Account;

public class AccessDeniedModel(UserManager<Person> userManager, SignInManager<Person> signInManager) : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        if (User.Identity is not { IsAuthenticated: true }) return Page();
        
        var user = await userManager.GetUserAsync(User);
        
        if (user == null) return Page();
        
        if (await userManager.IsInRoleAsync(user, "Administrador"))
        {
            return RedirectToPage("/Admin/Dashboard/Index"); 
        }
        
        if (await userManager.IsInRoleAsync(user, "Cliente"))
        {
            return RedirectToPage("/Cliente/Dashboard");
        }
        
        return Page();
    }
}