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

            string[] roleNames = { "Admin", "User" , "Certificador"};
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var user = await userManager.FindByEmailAsync("default@certificadodeauntencidad.com");

            if (user == null)
            {
                user = new IdentityUser()
                {
                    UserName = "default",
                    Email = "soporte@certificadodeauntencidad.com",
                };
                await userManager.CreateAsync(user, "Vigma@2024*");
            }
            await userManager.AddToRoleAsync(user, "Admin");

            var user2 = await userManager.FindByEmailAsync("default@certificadodeauntencidad.com");

            if (user2 == null)
            {
                user2 = new IdentityUser()
                {
                    UserName = "default",
                    Email = "default@certificadodeauntencidad.com",
                };
                await userManager.CreateAsync(user2, "Vigma@2024*");
            }
            await userManager.AddToRoleAsync(user2, "User");
        }
    }
}


