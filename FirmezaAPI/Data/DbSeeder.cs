using Firmeza.Data;
using Firmeza.Data.Entities;
using Firmeza.Models;
using Microsoft.AspNetCore.Identity;

namespace FirmezaAPI.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            UserManager<Person> userManager = serviceProvider.GetRequiredService<UserManager<Person>>();

            // Create roles if they don't exist
            string[] roleNames = { "Admin", "Client" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }

            // Create admin user if doesn't exist
            var adminEmail = "admin@firmeza.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new Client
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Administrator",
                    Document = "000000000",
                    Phone = "0000000000",
                    Age = 30,
                    RegisterDate = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            else
            {
                // Ensure existing admin has the Admin role
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
