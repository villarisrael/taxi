using Certificado2.Modelos;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Certificado2.Servicios
{
    public interface IRepositorioJoyeria
    {
        Task<Joyeria> ObtenerDatosJoyeria(int idJoyeria);
        Task<IEnumerable<Joyeria>> ObtenerListadoJoyeria();
        Task<IEnumerable<VJoyeria>> ObtenerListadoJoyeria(string certificador);
        Task<int> CrearCertificado(Joyeria joyeria);
    }

    public class RepositorioJoyeria : IRepositorioJoyeria
    {
        private readonly string connectionString;

        public RepositorioJoyeria(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConexionMySql");
        }

        public async Task<Joyeria> ObtenerDatosJoyeria(int idJoyeria)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM joyeria WHERE IdJoyeria = @idJoyeria";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@idJoyeria", idJoyeria);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Joyeria
                                {
                                    IdJoyeria = (int)reader["IdJoyeria"],
                                    Serie = reader["Serie"] as string,
                                    Folio = reader["Folio"] as int?,
                                    Objeto = reader["Objeto"] as string,
                                    Material = reader["Material"] as string,
                                    Estado = reader["Estado"] as string,
                                    Marca = reader["Marca"] as string,
                                    Observacion = reader["Observacion"] as string,
                                    fecha = reader["fecha"] is DBNull ? DateTime.MinValue : (DateTime)reader["fecha"],
                                    Foto = reader["Foto"] as byte[],
                                    idusucer = reader["idusucer"] as string,
                                    IdCertificador = (int)reader["IdCertificador"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine($"Error al obtener los datos de la joyería: {ex.Message}");
            }

            return null;
        }

        public async Task<IEnumerable<Joyeria>> ObtenerListadoJoyeria()
        {
            var listado = new List<Joyeria>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM joyeria ORDER BY IdJoyeria DESC LIMIT 200";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var joyeria = new Joyeria
                                {
                                    IdJoyeria = (int)reader["IdJoyeria"],
                                    Serie = reader["Serie"] as string,
                                    Folio = reader["Folio"] as int?,
                                    Objeto = reader["Objeto"] as string,
                                    Material = reader["Material"] as string,
                                    Estado = reader["Estado"] as string,
                                    Marca = reader["Marca"] as string,
                                    Observacion = reader["Observacion"] as string,
                                    fecha = reader["fecha"] is DBNull ? DateTime.MinValue : (DateTime)reader["fecha"],
                                    Foto = reader["Foto"] as byte[],
                                    idusucer = reader["idusucer"] as string,
                                    IdCertificador = (int)reader["IdCertificador"]
                                };

                                listado.Add(joyeria);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine($"Error al obtener listado de joyería: {ex.Message}");
            }

            return listado;
        }

        public async Task<IEnumerable<VJoyeria>> ObtenerListadoJoyeria(string certificador)
        {
            var listado = new List<VJoyeria>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM vjoyeria WHERE idusucer = @certificador ORDER BY IdJoyeria DESC LIMIT 200";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@certificador", "'" + certificador + "'");

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var joyeria = new VJoyeria
                                {
                                    IdCertificado = (int)reader["IdCertificado"],
                                    Serie = reader["Serie"] as string,
                                    Folio = (int)reader["Folio"] ,
                                    Objeto = reader["Objeto"] as string,
                                    Material = reader["Material"] as string,
                                    Estado = reader["Estado"] as string,
                                    Marca = reader["Marca"] as string,
                                    Observacion = reader["Observacion"] as string,
                                    fecha = reader["fecha"] is DBNull ? DateTime.MinValue : (DateTime)reader["fecha"],
                                    Foto = reader["Foto"] as byte[],
                                    idusucer = reader["idusucer"] as string,
                                    IdCertificador = (int)reader["IdCertificador"]
                                };

                                listado.Add(joyeria);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine($"Error al obtener listado de joyería por certificador: {ex.Message}");
            }

            return listado;
        }

        public async Task<int> CrearCertificado(Joyeria joyeria)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string insertQuery = @"
                    INSERT INTO joyeria 
                    (Serie, Folio, Objeto, Material, Estado, Marca, Observacion, fecha, Foto, idusucer, IdCertificador) 
                    VALUES 
                    (@Serie, @Folio, @Objeto, @Material, @Estado, @Marca, @Observacion, @fecha, @Foto, @idusucer, @IdCertificador);
                    SELECT LAST_INSERT_ID();";

                    using (var insertCommand = new MySqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Serie", joyeria.Serie);
                        insertCommand.Parameters.AddWithValue("@Folio", joyeria.Folio);
                        insertCommand.Parameters.AddWithValue("@Objeto", joyeria.Objeto);
                        insertCommand.Parameters.AddWithValue("@Material", joyeria.Material);
                        insertCommand.Parameters.AddWithValue("@Estado", joyeria.Estado);
                        insertCommand.Parameters.AddWithValue("@Marca", joyeria.Marca);
                        insertCommand.Parameters.AddWithValue("@Observacion", joyeria.Observacion);
                        insertCommand.Parameters.AddWithValue("@fecha", joyeria.fecha);
                        insertCommand.Parameters.AddWithValue("@Foto", joyeria.Foto);
                        insertCommand.Parameters.AddWithValue("@idusucer", joyeria.idusucer);
                        insertCommand.Parameters.AddWithValue("@IdCertificador", joyeria.IdCertificador);

                        int newId = Convert.ToInt32(await insertCommand.ExecuteScalarAsync());
                        return newId;
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine($"Error al crear la joyería: {ex.Message}");
                return -1;
            }
        }
    }
}
