
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Certificado2.Modelos;
using MySqlConnector;

using System;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace Certificado2.Servicios
{
    
    public interface IRepositorioArtesania
    {
        // Crear


        Task<int> CrearCertificado(Artesania artesania);
        
            // Leer (Obtener todos)
            Task<IEnumerable<Artesania>> GetAllArtesaniasAsync();

        // Leer (Obtener por ID)
        Task<Artesania> GetArtesaniaByIdAsync(int id);

        // Actualizar
        Task UpdateArtesaniaAsync(Artesania artesania);

        // Eliminar
        Task DeleteArtesaniaAsync(int id);
        Task<VArtesania> ObtenerArtesaniaPorFolio(string serie, int folio);
    }

  

public class RepositorioArtesania : IRepositorioArtesania
    {
        private readonly string connectionString;

        public RepositorioArtesania(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConexionMySql");
        }

        public async Task<int> CrearCertificado(Artesania artesania)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                   
                    string insertQuery = "INSERT INTO artesania (Fabricante, Serie, Folio, Descripcion, Materiales, Dimensiones, Peso, IDCertificador, Imagen, FechaCreacion, Fecha, Observacion, idusucer) " +
                                         "VALUES (@Fabricante, @Serie, @Folio, @Descripcion, @Materiales, @Dimensiones, @Peso, @IDCertificador, @Imagen, @FechaCreación, @Fecha, @Observacion, @idususer)";

                    
                        using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Fabricante", artesania.Fabricante);
                        command.Parameters.AddWithValue("@Serie", artesania.Serie);
                        command.Parameters.AddWithValue("@Folio", artesania.Folio);
                        command.Parameters.AddWithValue("@Descripcion", artesania.Descripcion);
                        command.Parameters.AddWithValue("@Materiales", artesania.Materiales);
                        command.Parameters.AddWithValue("@Dimensiones", artesania.Dimensiones);
                        command.Parameters.AddWithValue("@Peso", artesania.Peso);
                        command.Parameters.AddWithValue("@IDCertificador", artesania.IDCertificador);
                        command.Parameters.AddWithValue("@Imagen", artesania.Imagen);
                        command.Parameters.AddWithValue("@FechaCreación", artesania.FechaCreacion);
                        command.Parameters.AddWithValue("@Fecha", artesania.Fecha);
                        command.Parameters.AddWithValue("@Observacion", artesania.Observacion);
                        command.Parameters.AddWithValue("@idusucer", artesania.idusucer);
                        int newId = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return newId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar la artesanía: {ex.Message}");
                return -1;
            }
        }

        public async Task<IEnumerable<Artesania>> GetAllArtesaniasAsync()
        {
            var artesanias = new List<Artesania>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = "SELECT * FROM artesania order by Descripcion";

                    using (var command = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var artesania = new Artesania
                                {
                                    IDArtesania = (int)reader["IDArtesania"],
                                    Fabricante = reader["Fabricante"] as string,
                                    Serie = reader["Serie"] as string,
                                    Folio = (int)reader["Folio"],
                                    Descripcion = reader["Descripcion"] as string,
                                    Materiales = reader["Materiales"] as string,
                                    Dimensiones = reader["Dimensiones"] as string,
                                    Peso = reader["Peso"] as string,
                                    IDCertificador = (int)reader["IDCertificador"],
                                    Imagen = reader["Imagen"] as byte[],
                                    FechaCreacion = (DateTime)reader["FechaCreacion"],
                                    Fecha = (DateTime)reader["Fecha"],
                                    Observacion = reader["Observacion"] as string
                                };

                                artesanias.Add(artesania);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las artesanías: {ex.Message}");
            }

            return artesanias;
        }

        public async Task<Artesania> GetArtesaniaByIdAsync(int id)
        {
            Artesania artesania = null;

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = "SELECT * FROM artesania WHERE IDArtesania = @IDArtesania";

                    using (var command = new MySqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IDArtesania", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                artesania = new Artesania
                                {
                                    IDArtesania = (int)reader["IDArtesania"],
                                    Fabricante = reader["Fabricante"] as string,
                                    Serie = reader["Serie"] as string,
                                    Folio = (int)reader["Folio"],
                                    Descripcion = reader["Descripcion"] as string,
                                    Materiales = reader["Materiales"] as string,
                                    Dimensiones = reader["Dimensiones"] as string,
                                    Peso = reader["Peso"] as string,
                                    IDCertificador = (int)reader["IDCertificador"],
                                    Imagen = reader["Imagen"] as byte[],
                                    FechaCreacion = (DateTime)reader["FechaCreacion"],
                                    Fecha = (DateTime)reader["Fecha"],
                                    Observacion = reader["Observacion"] as string
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la artesanía por ID: {ex.Message}");
            }

            return artesania;
        }

        public async Task UpdateArtesaniaAsync(Artesania artesania)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string updateQuery = "UPDATE artesania SET Fabricante = @Fabricante, Serie = @Serie, Folio = @Folio, Descripcion = @Descripcion, Materiales = @Materiales, Dimensiones = @Dimensiones, Peso = @Peso, IDCertificador = @IDCertificador, Imagen = @Imagen, FechaCreación = @FechaCreación, Fecha = @Fecha, Observacion = @Observacion WHERE IDArtesania = @IDArtesania";

                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IDArtesania", artesania.IDArtesania);
                        command.Parameters.AddWithValue("@Fabricante", artesania.Fabricante);
                        command.Parameters.AddWithValue("@Serie", artesania.Serie);
                        command.Parameters.AddWithValue("@Folio", artesania.Folio);
                        command.Parameters.AddWithValue("@Descripcion", artesania.Descripcion);
                        command.Parameters.AddWithValue("@Materiales", artesania.Materiales);
                        command.Parameters.AddWithValue("@Dimensiones", artesania.Dimensiones);
                        command.Parameters.AddWithValue("@Peso", artesania.Peso);
                        command.Parameters.AddWithValue("@IDCertificador", artesania.IDCertificador);
                        command.Parameters.AddWithValue("@Imagen", artesania.Imagen);
                        command.Parameters.AddWithValue("@FechaCreación", artesania.FechaCreacion);
                        command.Parameters.AddWithValue("@Fecha", artesania.Fecha);
                        command.Parameters.AddWithValue("@Observacion", artesania.Observacion);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la artesanía: {ex.Message}");
            }
        }

        public async Task DeleteArtesaniaAsync(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string deleteQuery = "DELETE FROM artesania WHERE IDArtesania = @IDArtesania";

                    using (var command = new MySqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IDArtesania", id);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la artesanía: {ex.Message}");


            }
        }

        public async Task<VArtesania> ObtenerArtesaniaPorFolio(string serie, int folio)
        {
            VArtesania artesania = null;

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = "SELECT * FROM vartesania WHERE Serie = @Serie and Folio= @Folio";

                    using (var command = new MySqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Serie", serie);
                        command.Parameters.AddWithValue("@Folio", folio);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                artesania = new VArtesania
                                {
                                    IDArtesania = (int)reader["IDArtesania"],
                                    Fabricante = reader["Fabricante"] as string,
                                    Serie = reader["Serie"] as string,
                                    Folio = (int)reader["Folio"],
                                    Descripcion = reader["Descripcion"] as string,
                                    Materiales = reader["Materiales"] as string,
                                    Dimensiones = reader["Dimensiones"] as string,
                                    Peso = reader["Peso"] as string,
                                    IDCertificador = (int)reader["IDCertificador"],
                                    Imagen = reader["Imagen"] as byte[],
                                    FechaCreacion = (DateTime)reader["FechaCreacion"],
                                    Fecha = (DateTime)reader["Fecha"],
                                    Observacion = reader["Observacion"] as string,
                                    RazonSocial = reader["RazonSocial"] as string
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la artesanía por ID: {ex.Message}");
            }

            return artesania;
        }

    }
}
