using Certificado2.Modelos;
using MySqlConnector;
using System.Data;


namespace Certificado2.Servicios
{



    public interface IFoliosRepository
    {
        Task<int> GetNextFolioMonedaAsync();
        Task<int> GetNextFolioJoyeríaAsync();
        Task<int> GetNextFolioArtesaniaAsync();
        Task<string> GetSerieMonedaAsync();
        Task<string> GetSerieJoyeríaAsync();
        Task<string> GetSerieArtesaniaAsync();

        Task<FolioSiguiente> GetFolioArtesaniaAsync();

        Task<FolioSiguiente> GetFolioMonedaAsync();

        Task<FolioSiguiente> GetFolioJoyeriaAsync();

        Task ActualizaFolioMonedaAsync();

        Task ActualizaFolioJoyeriaAsync();

        Task ActualizaFolioArtesaniaAsync();
    }


    public class FolioSiguiente
    {
        public string Serie { get; set; }
        public int Folio { get; set; }
    }

    public class FoliosRepository : IFoliosRepository
    {
        private readonly string _connectionString;

        public FoliosRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionMySql");
        }

        public async Task<int> GetNextFolioMonedaAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("GetNextFolioMoneda", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<int> GetNextFolioJoyeríaAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("GetNextFolioJoyería", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<int> GetNextFolioArtesaniaAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("GetNextFolioArtesania", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<string> GetSerieMonedaAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("GetSerieMoneda", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var result = await command.ExecuteScalarAsync();
                    return result?.ToString();
                }
            }
        }

        public async Task<string> GetSerieJoyeríaAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("GetSerieJoyería", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var result = await command.ExecuteScalarAsync();
                    return result?.ToString();
                }
            }
        }

        public async Task<string> GetSerieArtesaniaAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("GetSerieArtesania", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var result = await command.ExecuteScalarAsync();
                    return result?.ToString();
                }
            }
        }

        public async Task<FolioSiguiente> GetFolioArtesaniaAsync()
        {
            FolioSiguiente folio = new FolioSiguiente();
            folio.Serie = await GetSerieArtesaniaAsync();
            folio.Folio = await GetNextFolioArtesaniaAsync();
            return folio;
        }

        public async Task<FolioSiguiente> GetFolioMonedaAsync()
        {
            FolioSiguiente folio = new FolioSiguiente();
            folio.Serie = await GetSerieMonedaAsync();
            folio.Folio = await GetNextFolioMonedaAsync();
            return folio;
        }

        public async Task<FolioSiguiente> GetFolioJoyeriaAsync()
        {
            FolioSiguiente folio = new FolioSiguiente();
            folio.Serie = await GetSerieJoyeríaAsync();
            folio.Folio = await GetNextFolioJoyeríaAsync();
            return folio;
        }

        public async Task ActualizaFolioMonedaAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(); // Usar await para abrir la conexión asincrónicamente

                try
                {
                    string updateQuery = @"UPDATE folios SET FolioMoneda = FolioMoneda + 1";
                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync(); // Usar await para ejecutar la consulta asincrónicamente
                    }
                }
                catch (Exception ex)
                {
                    // Loguear el error de manera adecuada en producción, como en un archivo de log o en un sistema de monitoreo.
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public async Task ActualizaFolioJoyeriaAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(); // Usar await para abrir la conexión asincrónicamente

                try
                {
                    string updateQuery = @"UPDATE folios SET FolioJoyeria = FolioJoyeria + 1";
                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync(); // Usar await para ejecutar la consulta asincrónicamente
                    }
                }
                catch (Exception ex)
                {
                    // Loguear el error de manera adecuada en producción, como en un archivo de log o en un sistema de monitoreo.
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public async Task ActualizaFolioArtesaniaAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(); // Usar await para abrir la conexión asincrónicamente

                try
                {
                    string updateQuery = @"UPDATE folios SET FolioArtesania = FolioArtesania + 1";
                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync(); // Usar await para ejecutar la consulta asincrónicamente
                    }
                }
                catch (Exception ex)
                {
                    // Loguear el error de manera adecuada en producción, como en un archivo de log o en un sistema de monitoreo.
                    Console.WriteLine(ex.ToString());
                }
            }
        }


    }
}
