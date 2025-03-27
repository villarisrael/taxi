using Microsoft.AspNetCore.Identity;
using Certificado2.Modelos;
using MySqlConnector;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Certificado2.Servicios
{
    public interface IRepositorioClientes
    {
        Task<bool> CrearClienteAsync(string Nombre, string Apellido, string Email, string Telefono, string Password);
        
    }

    public class RepositorioClientes : IRepositorioClientes
    {
        private readonly string connectionString;
        private readonly UserManager<UsuarioCertificados> _userManager;
        
        public RepositorioClientes(IConfiguration configuration, UserManager<UsuarioCertificados> userManager)
        {
            connectionString = configuration.GetConnectionString("ConexionMySql");
            _userManager = userManager;
            
        }

        public async Task<bool> CrearClienteAsync(string Nombre, string Apellido, string Email, string Telefono, string Password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(Email);

                if (user == null)
                {
                    user = new UsuarioCertificados()
                    {
                        UserName = Email,
                        NombreCompleto = Nombre + " "+ Apellido,
                        PhoneNumber = Telefono,
                        Email = Email,
                        idOrganizacion = 0
                    };

                    var result = await _userManager.CreateAsync(user, Password);

                    if (!result.Succeeded)
                    {
                        Console.WriteLine("Error al crear el usuario: " + string.Join(", ", result.Errors));
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                await _userManager.AddToRoleAsync(user, "Cliente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CrearClienteAsync: " + ex.Message);
                return false;
            }
        }
    }
}
