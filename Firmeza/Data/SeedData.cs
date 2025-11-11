using Firmeza.Data.Entities;
using Firmeza.Models;
using Microsoft.AspNetCore.Identity;

namespace Firmeza.Data;

public class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Person>>();
        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager);
    }
    private static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
    {
        string[] roleNames = ["Administrador", "Cliente"];
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }
    }
    private static async Task SeedAdminUserAsync(UserManager<Person> userManager)
    {
        const string adminEmail = "admin@firmeza.com";
        const string adminPassword = "Admin123!";
        var existingUser = await userManager.FindByEmailAsync(adminEmail);
        if (existingUser == null)

        {
            var adminUser = new Admin
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Administrador Principal",
                EmailConfirmed = true,
                LastLogin = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrador");
                Console.WriteLine($"Administrador '{adminEmail}' creado con éxito.");
            }
            else
            {
                Console.WriteLine($"Error al crear el administrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            Console.WriteLine($"El administrador '{adminEmail}' ya existe. Saltando Seed Data.");
        }
    }
}