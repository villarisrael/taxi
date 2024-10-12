using Certificado2.Modelos;


using MySqlConnector;


namespace Certificado2.Servicios
{
    public interface IRepositorioMonedas
    {
        Task<VMoneda> ObtenerDatosMoneda(string serie, int folio);
        Task<IEnumerable<VMoneda>> ObtenerListadoMoneda();
        Task<byte[]> ObtenerLogotipoEmpresaAsync();
        Task<IEnumerable<VMoneda>> ObtenerListadoMoneda(string certificador);

        Task<IEnumerable<VMoneda>> ObtenerListadoMoneda(string certificador, string moneda, string ano);

        Task<int> CrearCertificado(Moneda moneda);

    }

    public class RepositorioMonedas : IRepositorioMonedas
    {
        private readonly string connectionString;

        public RepositorioMonedas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConexionMySql");
        }

        public async Task<byte[]> ObtenerLogotipoEmpresaAsync()
        {
            byte[] logotipo = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT Logo_Empresa FROM empresa";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                logotipo = reader["Logo_Empresa"] as byte[];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine($"Error al obtener el logotipo de la empresa: {ex.Message}");
            }

            return logotipo;
        }

        public async Task<VMoneda> ObtenerDatosMoneda(string serie, int folio)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM Vmonedas WHERE SERIE = @serie AND FOLIO = @folio ";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@serie", serie);
                        selectCommand.Parameters.AddWithValue("@folio", folio);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new VMoneda
                                {
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Serie = reader["Serie"] as string,
                                    Folio = (int)reader["Folio"],
                                    Telefono = reader["Telefono"] as string,
                                    Nombre = reader["Nombre"] as string,
                                    Ano = reader["Ano"] as string,
                                    Ceca = reader["Ceca"] as string,
                                    Material = reader["Material"] as string,
                                    Estado = reader["Estado"] as string,
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
                // Loguear el error
                Console.WriteLine($"Error al obtener los datos de la moneda: {ex.Message}");
            }

            return null;
        }

        public async Task<IEnumerable<VMoneda>> ObtenerListadoMoneda()
        {
            var listado = new List<VMoneda>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM vmonedas ORDER BY idcertificado DESC LIMIT 200";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var moneda = new VMoneda
                                {
                                    idcertificado = (int)reader["idcertificado"],
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    RFC = reader["RFC"] as string,
                                    Serie = reader["Serie"] as string,
                                    Folio = (int)reader["Folio"],
                                    Nombre = reader["Nombre"] as string,
                                    Ano = reader["Ano"] as string,
                                    Ceca = reader["Ceca"] as string,
                                    Material = reader["Material"] as string,
                                    Estado = reader["Estado"] as string,
                                    fecha = reader["fecha"] is DBNull ? DateTime.MinValue : (DateTime)reader["fecha"],
                                    Foto = reader["Foto"] as byte[]
                                };

                                listado.Add(moneda);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine($"Error al obtener listado de certificaciones de moneda: {ex.Message}");
            }

            return listado;
        }

        public async Task<IEnumerable<VMoneda>> ObtenerListadoMoneda(string certificador)
        {
            var listado = new List<VMoneda>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM vmonedas where  razonsocial like '%"+certificador+ "%' or  razonsocial like '"+certificador+ "%' ORDER BY idcertificado DESC LIMIT 200";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var moneda = new VMoneda
                                {
                                    idcertificado = (int)reader["idcertificado"],
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    RFC = reader["RFC"] as string,
                                    Serie = reader["Serie"] as string,
                                    Folio = (int)reader["Folio"],
                                    Nombre = reader["Nombre"] as string,
                                    Ano = reader["Ano"] as string,
                                    Ceca = reader["Ceca"] as string,
                                    Material = reader["Material"] as string,
                                    Estado = reader["Estado"] as string,
                                    fecha = reader["fecha"] is DBNull ? DateTime.MinValue : (DateTime)reader["fecha"],
                                    Foto = reader["Foto"] as byte[]
                                };

                                listado.Add(moneda);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine($"Error al obtener listado de certificaciones de moneda: {ex.Message}");
            }

            return listado;
        }

        public async Task<IEnumerable<VMoneda>> ObtenerListadoMoneda(string certificador, string moneda, string ano)
        {
            var listado = new List<VMoneda>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM vmonedas where razonsocial like  '%"+ certificador + "%'  and Nombre like '%" +moneda+ "%'  and Ano like '%" +ano +"%'   ORDER BY idcertificado DESC LIMIT 200";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var vmoneda = new VMoneda
                                {
                                    idcertificado = (int)reader["idcertificado"],
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    RFC = reader["RFC"] as string,
                                    Serie = reader["Serie"] as string,
                                    Folio = (int)reader["Folio"],
                                    Nombre = reader["Nombre"] as string,
                                    Ano = reader["Ano"] as string,
                                    Ceca = reader["Ceca"] as string,
                                    Material = reader["Material"] as string,
                                    Estado = reader["Estado"] as string,
                                    fecha = reader["fecha"] is DBNull ? DateTime.MinValue : (DateTime)reader["fecha"],
                                    Foto = reader["Foto"] as byte[]
                                };

                                listado.Add(vmoneda);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine($"Error al obtener listado de certificaciones de moneda: {ex.Message}");
            }

            return listado;
        }


        public async Task<int> CrearCertificado(Moneda moneda)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string insertQuery = @"
                    INSERT INTO monedas 
                    ( Serie, Folio, IdCertificador, idusucer, Nombre, Ano, Ceca, Material, Estado, Foto, fecha) 
                    VALUES 
                    ( @Serie, @Folio, @IdCertificador, @idusucer, @Nombre, @Ano, @Ceca, @Material, @Estado, @Foto, @fecha);
                    SELECT LAST_INSERT_ID();";

                    using (var insertCommand = new MySqlCommand(insertQuery, connection))
                    {
         
                        insertCommand.Parameters.AddWithValue("@Serie", moneda.Serie);
                        insertCommand.Parameters.AddWithValue("@Folio", moneda.Folio);
                        insertCommand.Parameters.AddWithValue("@IdCertificador", moneda.IdCertificador);
                        insertCommand.Parameters.AddWithValue("@idusucer", moneda.idusucer);
                        insertCommand.Parameters.AddWithValue("@Nombre", moneda.Nombre);
                        insertCommand.Parameters.AddWithValue("@Ano", moneda.Ano);
                        insertCommand.Parameters.AddWithValue("@Ceca", moneda.Ceca);
                        insertCommand.Parameters.AddWithValue("@Material", moneda.Material);
                        insertCommand.Parameters.AddWithValue("@Estado", moneda.Estado);
                        insertCommand.Parameters.AddWithValue("@Foto", moneda.Foto);
                        insertCommand.Parameters.AddWithValue("@fecha", moneda.fecha);

                        int newId = Convert.ToInt32(await insertCommand.ExecuteScalarAsync());
                        return newId;
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine($"Error al crear el certificado: {ex.Message}");
                return -1;
            }
        }
    }
}
