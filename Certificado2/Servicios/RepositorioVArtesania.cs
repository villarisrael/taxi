using Certificado2.Modelos;
using MySqlConnector;
using System.Drawing;

namespace Certificado2.Servicios
{
    public interface IRepositorioVArtesania
    {
        Task<VArtesania> ObtenerArtesaniaPorId(int idArtesania);
        Task<IEnumerable<VArtesania>> ObtenerListadoArtesania();
        Task<VArtesania> ObtenerArtesaniaPorFolio(string serie, int folio);
        Task<IEnumerable<VArtesania>> ObtenerListadoArtesaniaxcertificador(int certificador, string buscar);

        Task<IEnumerable<VArtesania>> ObtenerListadoArtesaniaxcertificador(string certificador, string buscar);
    }

    public class RepositorioVArtesania : IRepositorioVArtesania
    {
        private readonly string connectionString;

        public RepositorioVArtesania(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConexionMySql");
        }

        public async Task<VArtesania> ObtenerArtesaniaPorId(int idArtesania)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM vartesania WHERE IDArtesania = @idArtesania";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@idArtesania", idArtesania);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new VArtesania
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
                Console.WriteLine($"Error al obtener los datos de la artesanía: {ex.Message}");
            }

            return null;
        }

        public async Task<IEnumerable<VArtesania>> ObtenerListadoArtesania()
        {
            var listado = new List<VArtesania>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM vartesania ORDER BY IDArtesania DESC LIMIT 200";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var artesania = new VArtesania
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

                                listado.Add(artesania);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener listado de artesanías: {ex.Message}");
            }

            return listado;
        }

        public async Task<VArtesania> ObtenerArtesaniaPorFolio(string serie, int folio)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM vartesania WHERE Serie = @serie AND Folio = @folio";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@serie", serie);
                        selectCommand.Parameters.AddWithValue("@folio", folio);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new VArtesania
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
                Console.WriteLine($"Error al obtener los datos de la artesanía: {ex.Message}");
            }

            return null;
        }

        public async Task<IEnumerable<VArtesania>> ObtenerListadoArtesaniaxcertificador(int certificador,string buscar)
        {
            var listado = new List<VArtesania>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = "";
                    if (buscar!="")
                    {
                        selectQuery = "SELECT * FROM vartesania WHERE IDCertificador = @certificador and Descripcion like @buscar ORDER BY IDArtesania DESC LIMIT 200";
                    }
                    else
                    {
                        selectQuery = "SELECT * FROM vartesania WHERE IDCertificador = @certificador  ORDER BY IDArtesania DESC LIMIT 200";
                    }
                    

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@certificador", certificador);
                        selectCommand.Parameters.AddWithValue("@buscar", "%" + buscar + "%");

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var artesania = new VArtesania
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

                                listado.Add(artesania);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener listado de artesanías por certificador: {ex.Message}");
            }

            return listado;
        }

        public async Task<IEnumerable<VArtesania>> ObtenerListadoArtesaniaxcertificador(string certificador, string buscar)
        {
            var listado = new List<VArtesania>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = "";
                    if (buscar != "")
                    {
                        selectQuery = "SELECT * FROM vartesania WHERE (fabricante like @certificador  or RazonSocial like @certificador) and Descripcion like @buscar ORDER BY IDArtesania DESC LIMIT 200";
                    }
                    else
                    {
                        selectQuery = "SELECT * FROM vartesania WHERE (fabricante like @certificador or RazonSocial like @certificador)  ORDER BY IDArtesania DESC LIMIT 200";
                    }


                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@certificador", "%" + certificador + "%");
                        selectCommand.Parameters.AddWithValue("@buscar", "%" + buscar + "%");

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var artesania = new VArtesania
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

                                listado.Add(artesania);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener listado de artesanías por certificador: {ex.Message}");
            }

            return listado;
        }
    }
}
