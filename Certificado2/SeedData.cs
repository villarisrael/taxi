using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Certificado2
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var user = await userManager.FindByEmailAsync("soporte@vigmaconsultores.com");

            if (user == null)
            {
                user = new IdentityUser()
                {
                    UserName = "admin",
                    Email = "soporte@vigmaconsultores.com",
                };
                await userManager.CreateAsync(user, "Vigma@2024*");
            }
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}


