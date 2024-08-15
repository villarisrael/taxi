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

    }
}
