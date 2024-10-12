using Certificado2.Modelos;

using MySqlConnector;


namespace Certificado2.Servicios
{



    public interface IRepositorioVJoyeria
    {
        Task<VJoyeria> ObtenerJoyeriaPorId(int idCertificado);
        Task<IEnumerable<VJoyeria>> ObtenerListadoJoyeria();
        Task<VJoyeria> ObtenerJoyeriaPorFolio(string serie, int folio);
        Task<IEnumerable<VJoyeria>> ObtenerListadoJoyeria(int certificador, string searchstring);

        Task<IEnumerable<VJoyeria>> ObtenerListadoJoyeriacertificador(string certificador, string searchstring);
    }


        public class RepositorioVJoyeria : IRepositorioVJoyeria
        {
            private readonly string connectionString;

            public RepositorioVJoyeria(IConfiguration configuration)
            {
                connectionString = configuration.GetConnectionString("ConexionMySql");
            }

            public async Task<VJoyeria> ObtenerJoyeriaPorId(int idCertificado)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        string selectQuery = "SELECT * FROM vjoyeria WHERE idcertificado = @idCertificado";

                        using (var selectCommand = new MySqlCommand(selectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@idCertificado", idCertificado);

                            using (var reader = await selectCommand.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    return new VJoyeria
                                    {
                                        IdCertificado = (int)reader["idcertificado"],
                                        RazonSocial = reader["RazonSocial"] as string,
                                        NombreResponsable = reader["NombreResponsable"] as string,
                                        Telefono = reader["Telefono"] as string,
                                        RFC = reader["RFC"] as string,
                                        Serie = reader["Serie"] as string,
                                        Folio = (int)reader["Folio"],
                                        Objeto = reader["Objeto"] as string,
                                        Material = reader["Material"] as string,
                                        Estado = reader["Estado"] as string,
                                        Marca = reader["Marca"] as string,
                                        Observacion = reader["observacion"] as string,
                                        fecha = reader["fecha"] is DBNull ? DateTime.MinValue : (DateTime)reader["fecha"],
                                        Foto = reader["Foto"] as byte[]
                                    };
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener los datos de la joyería: {ex.Message}");
                }

                return null;
            }

            public async Task<IEnumerable<VJoyeria>> ObtenerListadoJoyeria()
            {
                var listado = new List<VJoyeria>();

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        string selectQuery = "SELECT * FROM vjoyeria ORDER BY idcertificado DESC LIMIT 200";

                        using (var selectCommand = new MySqlCommand(selectQuery, connection))
                        {
                            using (var reader = await selectCommand.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var joyeria = new VJoyeria
                                    {
                                        IdCertificado = (int)reader["idcertificado"],
                                        RazonSocial = reader["RazonSocial"] as string,
                                        NombreResponsable = reader["NombreResponsable"] as string,
                                        Telefono = reader["Telefono"] as string,
                                        RFC = reader["RFC"] as string,
                                        Serie = reader["Serie"] as string,
                                        Folio = (int)reader["Folio"],
                                        Objeto = reader["Objeto"] as string,
                                        Material = reader["Material"] as string,
                                        Estado = reader["Estado"] as string,
                                        Marca = reader["Marca"] as string,
                                        Observacion = reader["observacion"] as string,
                                        fecha = reader["fecha"] is DBNull ? DateTime.MinValue : (DateTime)reader["fecha"],
                                        Foto = reader["Foto"] as byte[]
                                    };

                                    listado.Add(joyeria);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener listado de joyería: {ex.Message}");
                }

                return listado;
            }


        public async Task<VJoyeria> ObtenerJoyeriaPorFolio(string serie, int folio)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM vjoyeria WHERE serie = @serie and Folio=@Folio";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@serie", serie);
                        selectCommand.Parameters.AddWithValue("@Folio", folio);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new VJoyeria
                                {
                                    IdCertificado = (int)reader["idcertificado"],
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    RFC = reader["RFC"] as string,
                                    Serie = reader["Serie"] as string,
                                    Folio = (int)reader["Folio"],
                                    Objeto = reader["Objeto"] as string,
                                    Material = reader["Material"] as string,
                                    Estado = reader["Estado"] as string,
                                    Marca = reader["Marca"] as string,
                                    Observacion = reader["observacion"] as string,
                                    fecha = reader["fecha"] is DBNull ? DateTime.MinValue : (DateTime)reader["fecha"],
                                    Foto = reader["Foto"] as byte[]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los datos de la joyería: {ex.Message}");
            }

            return null;
        }

        public async Task<IEnumerable<VJoyeria>> ObtenerListadoJoyeria(int certificador, string searchstring)
        {
            var listado = new List<VJoyeria>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery;
                    if (!string.IsNullOrEmpty(searchstring))
                    {
                        selectQuery = "SELECT * FROM vjoyeria WHERE idCertificador = @certificador AND (objeto LIKE @searchstring) ORDER BY Idcertificado DESC LIMIT 200";
                    }
                    else
                    {
                        selectQuery = "SELECT * FROM vjoyeria WHERE idCertificador = @certificador ORDER BY Idcertificado DESC LIMIT 200";
                    }

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        // Agregar parámetros a la consulta
                        selectCommand.Parameters.AddWithValue("@certificador", certificador);
                        selectCommand.Parameters.AddWithValue("@searchstring", "%" + searchstring + "%"); // Para LIKE se usa '%' antes y después del valor




                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var joyeria = new VJoyeria
                                {
                                    IdCertificado = (int)reader["IdCertificado"],
                                    Serie = reader["Serie"] as string,
                                    Folio = (int)reader["Folio"],
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
            public async Task<IEnumerable<VJoyeria>> ObtenerListadoJoyeriacertificador(string certificador, string searchstring)
            {
                var listado = new List<VJoyeria>();

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        string selectQuery;
                        if (!string.IsNullOrEmpty(searchstring))
                        {
                            selectQuery = "SELECT * FROM vjoyeria WHERE (razonsocial like  @certificador ) AND (objeto LIKE @searchstring) ORDER BY Idcertificado DESC LIMIT 200";
                        }
                        else
                        {
                            selectQuery = "SELECT * FROM vjoyeria WHERE razonsocial like  @certificador or  ORDER BY Idcertificado DESC LIMIT 200";
                        }

                        using (var selectCommand = new MySqlCommand(selectQuery, connection))
                        {
                            // Agregar parámetros a la consulta
                          
                            selectCommand.Parameters.AddWithValue("@searchstring", "%" + searchstring + "%"); // Para LIKE se usa '%' antes y después del valor
                            selectCommand.Parameters.AddWithValue("@certificador", "%" + certificador + "%");



                            using (var reader = await selectCommand.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var joyeria = new VJoyeria
                                    {
                                        IdCertificado = (int)reader["IdCertificado"],
                                        Serie = reader["Serie"] as string,
                                        Folio = (int)reader["Folio"],
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
                    return listado; 
                }
                catch (Exception ex)
                {
                    // Loguear el error
                    Console.WriteLine($"Error al obtener listado de joyería por certificador: {ex.Message}");
                }

                return listado;
        }
    }

 
}
