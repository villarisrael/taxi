using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Certificado2
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<UsuarioCertificados> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "Organizacion", "Conductor", "Cliente" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"❌ Error al crear rol {roleName}:");
                        foreach (var error in roleResult.Errors)
                        {
                            Console.WriteLine($"  - {error.Description}");
                        }
                    }
                }
            }

            // Crear usuarios con manejo de errores detallado
           await CrearUsuario(userManager, "conductor1@taxi.com", "Conductor1", "Conductor pruebna", "Con_1234", "Conductor");
            await CrearUsuario(userManager, "Organizacion1@taxi.com", "Organizacion1", "Organizacion pruebna", "Org_1234", "Organizacion");
            await CrearUsuario(userManager, "webmaster@taxi.com", "Admin", "ISRAEL VILLAR MEDINA", "Vigma2024", "Admin");
        }

      public static async Task CrearUsuario(UserManager<UsuarioCertificados> userManager, string email, string username, string nombreCompleto, string password, string rol)
        {
            try
            {
                var usuario = await userManager.FindByEmailAsync(email);

                if (usuario == null)
                {
                    usuario = new UsuarioCertificados()
                    {
                        UserName = username,
                        Email = email,
                        NombreCompleto = nombreCompleto
                    };

                    var createResult = await userManager.CreateAsync(usuario, password);

                    if (!createResult.Succeeded)
                    {
                        Console.WriteLine($"❌ Error al crear usuario {email}:");
                        foreach (var error in createResult.Errors)
                        {
                            Console.WriteLine($"  - {error.Description}");
                        }
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"✅ Usuario {email} ya existe.");
                }

                // Verificar si el usuario ya está en el rol antes de asignarlo
                if (!await userManager.IsInRoleAsync(usuario, rol))
                {
                    var roleResult = await userManager.AddToRoleAsync(usuario, rol);
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"❌ Error al asignar rol {rol} al usuario {email}:");
                        foreach (var error in roleResult.Errors)
                        {
                            Console.WriteLine($"  - {error.Description}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"✅ Usuario {email} agregado al rol {rol}.");
                    }
                }
                else
                {
                    Console.WriteLine($"✅ Usuario {email} ya tiene el rol {rol}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción al crear usuario {email}: {ex.Message}");
            }
        }
    }
}
