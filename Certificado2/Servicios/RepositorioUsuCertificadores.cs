using System;
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


        public bool VerificarCredenciales(string usuario, string contraseña)
        {
            bool esValido = false;

            // Consulta SQL para verificar las credenciales
            string consulta = "SELECT COUNT(*) FROM UsuCertificadores WHERE Usuario = @Usuario AND Password = @Contraseña";

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                MySqlCommand comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Usuario", usuario);
                comando.Parameters.AddWithValue("@Contraseña", contraseña);

                try
                {
                    conexion.Open();
                    int cantidad = Convert.ToInt32(comando.ExecuteScalar());
                    esValido = cantidad > 0;
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
