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
        Task<bool> CrearClienteAsync(string nombre, string username, string telefono, string pass, int idOrganizacion = 0);
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

        public async Task<bool> CrearClienteAsync(string nombre, string username, string telefono, string pass, int idOrganizacion = 0)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(username);

                if (user == null)
                {
                    user = new UsuarioCertificados()
                    {
                        UserName = username,
                        NombreCompleto = nombre,
                        //Telefono = telefono,
                        Email = username,
                        idOrganizacion = idOrganizacion
                    };

                    var result = await _userManager.CreateAsync(user, pass);

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

                await _userManager.AddToRoleAsync(user, "Conductor");
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
