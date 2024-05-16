using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MySqlConnector;

namespace Certificado2.Modelos
{
    public class RepositorioUsuCertificadores
    {
        private string _cadenaConexion;

        public interface IRepositorioUsuCertificadores
        {
            Task<IEnumerable<UsuCertificadores>> ObtenerListado();
            Task CrearAsync(UsuCertificadores certificador);
            Task ModificarAsync(UsuCertificadores certificador);
          
           
            Task<UsuCertificadores> ObtenerDetalleAsync(int id);
        }



        public RepositorioUsuCertificadores(IConfiguration configuration)
        {
            _cadenaConexion =  configuration.GetConnectionString("ConexionMySql"); ;
        }


       
        public async Task<IEnumerable<UsuCertificadores>> ObtenerListado()
        {
            var listaCertificadores = new List<UsuCertificadores>();

            try
            {
                using (var conexion = new MySqlConnection(_cadenaConexion))
                {
                    await conexion.OpenAsync();

                    string consulta = "SELECT * FROM usucertificadores";

                    using (var comando = new MySqlCommand(consulta, conexion))
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            var certificador = new UsuCertificadores
                            {
                                Id = Convert.ToInt32(lector["idusucertificadores"]),
                                Nombre = lector["Nombre"] as string,
                                Usuario = lector["usuario"] as string,
                                Password = lector["password"] as string,
                                Email = lector["email"] as string,
                                WhatsApp = lector["whatsapp"] as string,
                                IdCentificador = lector["idcentificador"] as int? // Assuming nullable int
                            };

                            listaCertificadores.Add(certificador);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener listado de certificadores: {ex.Message}");
            }

            return listaCertificadores;
        }

        public async Task CrearAsync(UsuCertificadores certificador)
        {
            try
            {
                using (var conexion = new MySqlConnection(_cadenaConexion))
                {
                    await conexion.OpenAsync();

                    string consulta = "INSERT INTO usucertificadores (Nombre, usuario, password, email, whatsapp, idcentificador) " +
                                      "VALUES (@Nombre, @Usuario, @Password, @Email, @WhatsApp, @IdCentificador)";

                    using (var comando = new MySqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", certificador.Nombre);
                        comando.Parameters.AddWithValue("@Usuario", certificador.Usuario);
                        comando.Parameters.AddWithValue("@Password", certificador.Password);
                        comando.Parameters.AddWithValue("@Email", certificador.Email);
                        comando.Parameters.AddWithValue("@WhatsApp", certificador.WhatsApp);
                        comando.Parameters.AddWithValue("@IdCentificador", certificador.IdCentificador);

                        await comando.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear certificador: {ex.Message}");
            }
        }

        public async Task ModificarAsync(UsuCertificadores certificador)
        {
            try
            {
                using (var conexion = new MySqlConnection(_cadenaConexion))
                {
                    await conexion.OpenAsync();

                    string consulta = "UPDATE usucertificadores SET Nombre = @Nombre, usuario = @Usuario, " +
                                      "password = @Password, email = @Email, whatsapp = @WhatsApp, " +
                                      "idcentificador = @IdCentificador WHERE idusucertificadores = @Id";

                    using (var comando = new MySqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", certificador.Nombre);
                        comando.Parameters.AddWithValue("@Usuario", certificador.Usuario);
                        comando.Parameters.AddWithValue("@Password", certificador.Password);
                        comando.Parameters.AddWithValue("@Email", certificador.Email);
                        comando.Parameters.AddWithValue("@WhatsApp", certificador.WhatsApp);
                        comando.Parameters.AddWithValue("@IdCentificador", certificador.IdCentificador);
                        comando.Parameters.AddWithValue("@Id", certificador.Id);

                        await comando.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar certificador: {ex.Message}");
            }
        }


        public async Task<bool> VerificarCredencialesAsync(string usuario, string contraseña)


        {


            bool esValido = false;
            UsuCertificadores certificador = null;

            // Consulta SQL para verificar las credenciales
            string consulta = "SELECT * FROM usucertificadores WHERE  Usuario = @Usuario AND Password = @Contraseña";

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                MySqlCommand comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Usuario", usuario);
                comando.Parameters.AddWithValue("@Contraseña", contraseña);

                try
                {
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        if (await lector.ReadAsync())
                        {
                            certificador = new UsuCertificadores
                            {
                                Id = Convert.ToInt32(lector["idusucertificadores"]),
                                Nombre = lector["Nombre"] as string,
                                Usuario = lector["usuario"] as string,
                                Password = lector["password"] as string,
                                Email = lector["email"] as string,
                                WhatsApp = lector["whatsapp"] as string,
                                IdCentificador = lector["idcentificador"] as int ? // Assuming nullable int
                              
                            };
                            esValido = true;
                        }
                    }

                    if (esValido)
                    {
                        // Si las credenciales son válidas, establecer la identidad y el usuario autenticado
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, certificador.Id.ToString()),
                            new Claim(ClaimTypes.Name, certificador.Nombre),
                             new Claim(ClaimTypes.Email, certificador.Email),
                             new Claim(ClaimTypes.Actor, certificador.IdCentificador.ToString()),
                            new Claim(ClaimTypes.Role, "Certificador") // Establecer el rol manualmente
                        };

                        var identity = new ClaimsIdentity(claims, "login");
                        var principal = new ClaimsPrincipal(identity);

                        // Acceder al contexto HTTP actual
                        var httpContext = new DefaultHttpContext { User = principal };  // SE INVOCA LA VARIBALE User para acceder a los datos del logeo
                        var authenticationService = httpContext.RequestServices.GetRequiredService<IAuthenticationService>();

                        AuthenticationProperties authenticationProperties = null;

                        string scheme = "Identity.Application";

                        // Opciones adicionales para la autenticación (en este caso, nulo)
                  
                        // Iniciar sesión
                        await authenticationService.SignInAsync(httpContext, scheme, principal, authenticationProperties);
                        // Iniciar sesión
                
                    
                }
                }
                catch (Exception ex)
                {
                    // Manejar excepción
                    Console.WriteLine("Error al verificar las credenciales: " + ex.Message);
                }
            }

            return esValido;
        }

        public async Task<UsuCertificadores> ObtenerDetalleAsync(int id)
        {
            UsuCertificadores certificador = null;

            try
            {
                using (var conexion = new MySqlConnection(_cadenaConexion))
                {
                    await conexion.OpenAsync();

                    string consulta = "SELECT * FROM usucertificadores WHERE idusucertificadores = @Id";

                    using (var comando = new MySqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@Id", id);

                        using (var lector = await comando.ExecuteReaderAsync())
                        {
                            if (await lector.ReadAsync())
                            {
                                certificador = new UsuCertificadores
                                {
                                    Id = Convert.ToInt32(lector["idusucertificadores"]),
                                    Nombre = lector["Nombre"] as string,
                                    Usuario = lector["usuario"] as string,
                                    Password = lector["password"] as string,
                                    Email = lector["email"] as string,
                                    WhatsApp = lector["whatsapp"] as string,
                                    IdCentificador = lector["idcentificador"] as int? // Assuming nullable int
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalle del certificador: {ex.Message}");
            }

            return certificador;
        }

    }
}
